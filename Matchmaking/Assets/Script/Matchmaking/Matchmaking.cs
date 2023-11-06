using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

public class Matchmaking
{
    public static List<Rank> list_of_ranks = new List<Rank>
    {
        new Rank(0, 249,"Rank 1"),
        new Rank(250, 499, "Rank 2"),
        new Rank(500, 749, "Rank 3"),
        new Rank(750, 999, "Rank 4"),
        new Rank(1000, 1249, "Rank 5"),
        new Rank(1250, 1449, "Rank 6"),
        new Rank(1500, 1749, "Rank 7"),
        new Rank(1750, 1999, "Rank 8")
    };

    public static Queue<Ticket>[] queue_tickets { get; set; }
    public static List<Match> matches = new List<Match>();
    public static void Initialization()
    {
        queue_tickets = new Queue<Ticket>[list_of_ranks.Count];
        for (int i = 0; i < list_of_ranks.Count; i++)
            queue_tickets[i] = new Queue<Ticket>();
    }

    public static void CreateTicketRequest(Player player)
    {
        Ticket ticket = new Ticket(player);

        int tickets_position = GetTicketsPosition(ticket.player.data.rank);
        queue_tickets[tickets_position].Enqueue(ticket);

        NetCreateTicket responess = new NetCreateTicket();
        responess.class_type = player.data.class_type;
        Sender.SendToClient_Reliable(player.connection_id, responess);
    }

    public static void FindMatchRequest(Player player)
    {
        if (player.ticket_reference == null)
            return;

        int tickets_position = GetTicketsPosition(player.data.rank);
        Ticket player_ticket = queue_tickets[tickets_position]
        .FirstOrDefault(ticket => ticket.ticket_id == player.ticket_reference.ticket_id);        

        if (player_ticket == null)
        {
            UnityEngine.Debug.Log("PLAYER TICKET DOESNT EXIST !!!");
            return;
        }

        if (queue_tickets[tickets_position].Count > 1)
        {
            Ticket opponent_ticket = queue_tickets[tickets_position].FirstOrDefault(ticket =>
            ticket != player_ticket && player_ticket.player.data.class_type != ticket.player.data.class_type);

            if (opponent_ticket != null)
            {
                CreateMatch(player, opponent_ticket.player);

                queue_tickets[tickets_position] = new Queue<Ticket>(queue_tickets[tickets_position]
                .Where(ticket => ticket.ticket_id != player.ticket_reference.ticket_id && ticket.ticket_id != opponent_ticket.ticket_id));

                opponent_ticket.player.ticket_reference = null;
                player.ticket_reference = null;
            }
        }
    }

    public static void DeleteTicket(Player player)
    {
        if (player.ticket_reference == null)
            return;

        int tickets_position = GetTicketsPosition(player.data.rank);

        queue_tickets[tickets_position] = new Queue<Ticket>(queue_tickets[tickets_position]
            .Where(ticket => ticket.ticket_id != player.ticket_reference.ticket_id));

        player.ticket_reference = null;

        Sender.SendToClient_Reliable(player.connection_id, new NetStopMatchFinding());
    }
    public static async void AcceptMatch(Player player)
    {
        Match match = player.match_reference;
        if(match != null)
        {
            match.Accept(player);
            if(match.IsReady() && match.players.Count == 2)
            {
                Player player1 = match.players.ElementAt(0).Key;
                Player player2 = match.players.ElementAt(1).Key;

                int match_id = await Database.CreateMatchAsync(player1, player2);
                await Database.UpdatePlayerClassType(player1);
                await Database.UpdatePlayerClassType(player2);

                NetMatchCreated responess = new NetMatchCreated();
                responess.match_id = match_id;
                responess.ip_address = NetworkManager.GetExternalIP();
                //responess.ip_address = NetworkManager.GetInternalIP(AddressFamily.InterNetwork);
                responess.port = 27001;

                Sender.SendToClient_Reliable(player1.connection_id, responess);
                Sender.SendToClient_Reliable(player2.connection_id, responess);

                player1.match_reference = null;
                player2.match_reference = null;

                //if match has string id remove from list matches
                //match = null;
            }

        }
    }
    public static void DeclineMatch(Player player)
    {
        if (player.match_reference == null)
            return;

        if (!string.IsNullOrWhiteSpace(player.match_reference.loby_id))
            matches.Remove(player.match_reference);

        player.match_reference.DeclineMatch(player);


    }

    public static void CreateLoby(Player player)
    {
        Match match = new Match(player);
        matches.Add(match);

        NetCreateLoby responess = new NetCreateLoby()
        {
            class_type = player.data.class_type,
            loby_id = match.loby_id,
        };
        Sender.SendToClient_Reliable(player.connection_id, responess);
    }

    public static void JoinLoby(Player player, string code)
    {
        Match match = matches.FirstOrDefault(m => m.loby_id == code);
        if(match != null)
        {
            match.players.Add(player, false);
            player.match_reference = match;
            Player opponent = match.players.FirstOrDefault(p => p.Value == true).Key;
            if(opponent != null)
            {
                if (opponent.data.class_type == ClassType.Light)
                    player.data.class_type = ClassType.Dark;
                else
                    player.data.class_type = ClassType.Light;

                NetFindMatch response = new NetFindMatch
                {
                    enemy_data = opponent.data
                };
                Sender.SendToClient_Reliable(player.connection_id, response);
            }
        }
        else
        {
            //loby is destroyed
        }
    }
    private static void CreateMatch(Player player1, Player player2)
    {
        new Match(player1, player2);

        NetFindMatch response1 = new NetFindMatch
        {
            enemy_data = player2.data
        };

        NetFindMatch response2 = new NetFindMatch
        {
            enemy_data = player1.data
        };

        Sender.SendToClient_Reliable(player1.connection_id, response1);
        Sender.SendToClient_Reliable(player2.connection_id, response2);
    }

    public static int GetTicketsPosition(int mmr)
    {
        int position = -1;
        foreach (Rank rank in list_of_ranks)
        {
            ++position;
            if (rank.min <= mmr && mmr <= rank.max)
            {
                break;
            }
        }
        return position;
    }
}
public class Match
{
    public string loby_id { get; set; }
    public Dictionary<Player, bool> players { get; set; }
    public Match(Player player1, Player player2)
    {
        player1.match_reference = this;
        player2.match_reference = this;
        players = new Dictionary<Player, bool>() { { player1, false }, { player2, false } };
    }
    public Match(Player player1)
    {
        loby_id = Guid.NewGuid().ToString().Substring(0, 8);
        player1.match_reference = this;
        players = new Dictionary<Player, bool>() { { player1, true } };
    }

    public void Accept(Player player)
    {
        players[player] = true;
        NetAcceptMatch responess = new NetAcceptMatch()
        {
            class_type = player.data.class_type,
        };

        foreach (Player p in players.Keys)
            Sender.SendToClient_Reliable(p.connection_id,responess);

        player.match_reference = null;
    }

    public void DeclineMatch(Player player)
    {
        NetDeclineMatch responess = new NetDeclineMatch();
        responess.class_type = player.data.class_type;

        foreach (Player p in players.Keys)
            Sender.SendToClient_Reliable(p.connection_id, responess);

        players = null;
    }

    public bool IsReady()
    {
        return players.All(player => player.Value);
    }
}
public class Ticket
{
    public string ticket_id { get; set; }
    public Player player { get; set; }
    public int give_up { get; set; }

    public Ticket(Player _player)
    {
        player = _player;
        player.ticket_reference = this;
        ticket_id = Guid.NewGuid().ToString();
    }

}

public class Rank
{
    public int min { get; set; }
    public int max { get; set; }
    public string name { get; set; }

    public Rank(int _min, int _max, string _name)
    {
        min = _min;
        max = _max;
        name = _name;
    }
}
