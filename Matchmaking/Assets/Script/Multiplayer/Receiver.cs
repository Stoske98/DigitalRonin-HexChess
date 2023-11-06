using Riptide;
using System;
using System.Net.Sockets;

public class Receiver
{
    public void Subscibe()
    {
        NetworkManager.S_ON_KEEP_ALIVE_REQUEST += OnKeepAliveRequest;
        NetworkManager.S_ON_AUTH_REQUEST += OnAuthenticationRequest;
        NetworkManager.S_ON_CHANGE_NICKNAME_REQUEST += OnChangeNicknameRequest;
        NetworkManager.S_ON_CREATE_TICKET_REQUEST += OnCreateTicketRequest;
        NetworkManager.S_ON_FIND_MATCH_REQUEST += OnFindMatchRequest;
        NetworkManager.S_ON_STOP_MATCH_FINDING_REQUEST += OnStopFindingMatchRequest;
        NetworkManager.S_ON_ACCEPT_MATCH_REQUEST += OnAcceptMatchRequest;
        NetworkManager.S_ON_DECLINE_MATCH_REQUEST += OnDeclineMatchRequest;
        NetworkManager.S_ON_CREATE_LOBY_REQUEST += OnCreateLobyRequest;
        NetworkManager.S_ON_JOIN_LOBY_REQUEST += OnJoinLobyRequest;
    }

    public void UnSubscibe()
    {
        NetworkManager.S_ON_KEEP_ALIVE_REQUEST -= OnKeepAliveRequest;
        NetworkManager.S_ON_AUTH_REQUEST -= OnAuthenticationRequest;
        NetworkManager.S_ON_CHANGE_NICKNAME_REQUEST -= OnChangeNicknameRequest;
        NetworkManager.S_ON_CREATE_TICKET_REQUEST -= OnCreateTicketRequest;
        NetworkManager.S_ON_FIND_MATCH_REQUEST -= OnFindMatchRequest;
        NetworkManager.S_ON_STOP_MATCH_FINDING_REQUEST += OnStopFindingMatchRequest;
        NetworkManager.S_ON_ACCEPT_MATCH_REQUEST -= OnAcceptMatchRequest;
        NetworkManager.S_ON_DECLINE_MATCH_REQUEST -= OnDeclineMatchRequest;
        NetworkManager.S_ON_DECLINE_MATCH_REQUEST -= OnDeclineMatchRequest;
        NetworkManager.S_ON_CREATE_LOBY_REQUEST -= OnCreateLobyRequest;
        NetworkManager.S_ON_JOIN_LOBY_REQUEST -= OnJoinLobyRequest;
    }


    private void OnKeepAliveRequest(NetMessage message, Connection connection)
    {
        
    }

    private async void OnAuthenticationRequest(NetMessage message, Connection connection)
    {
        NetAuthentication request = message as NetAuthentication;
        PlayerData _player_data = await Database.AuthenticatePlayerAsync(request.device_id);

        if (_player_data == null)
        {   NetworkManager.Instance.DisconnectPlayer(connection);
            return;
        }

        Player player = NetworkManager.Instance.players[connection.Id];
        player.device_id = request.device_id;
        player.data = _player_data;

        int match_id = await Database.CheckIsThereAGameThatIsNotFinished(NetworkManager.Instance.players[connection.Id]);

        if (match_id == 0)
        {
            NetLogin responess = new NetLogin()
            {
                player_data = _player_data,
            };

            Sender.SendToClient_Reliable(connection.Id, responess);

        }
        else
        {
            NetGameExist responess = new NetGameExist()
            {
                match_id = match_id,
                //ip_address = NetworkManager.GetInternalIP(AddressFamily.InterNetwork),
                ip_address = NetworkManager.GetExternalIP(),
                port = 27001
            };

            Sender.SendToClient_Reliable(connection.Id, responess);
        }       
    }

    private async void OnChangeNicknameRequest(NetMessage message, Connection connection)
    {
        NetChangeNickname request = message as NetChangeNickname;
        await Database.UpdatePlayerNickname(request.nickname, NetworkManager.Instance.players[connection.Id]);

        Sender.SendToClient_Reliable(connection.Id, request);
    }

    private void OnCreateTicketRequest(NetMessage message, Connection connection)
    {
        NetCreateTicket request = message as NetCreateTicket;
        Player player = NetworkManager.Instance.players[connection.Id];
        player.data.class_type = request.class_type;

        Matchmaking.CreateTicketRequest(NetworkManager.Instance.players[connection.Id]);
    }

    private void OnFindMatchRequest(NetMessage message, Connection connection)
    {
        Matchmaking.FindMatchRequest(NetworkManager.Instance.players[connection.Id]);
    }

    private void OnStopFindingMatchRequest(NetMessage message, Connection connection)
    {
        Matchmaking.DeleteTicket(NetworkManager.Instance.players[connection.Id]);
    }

    private void OnDeclineMatchRequest(NetMessage message, Connection connection)
    {
        Matchmaking.DeclineMatch(NetworkManager.Instance.players[connection.Id]);
    }

    private void OnAcceptMatchRequest(NetMessage message, Connection connection)
    {
        Matchmaking.AcceptMatch(NetworkManager.Instance.players[connection.Id]);
    }

    private void OnCreateLobyRequest(NetMessage message, Connection connection)
    {
        NetCreateLoby request = message as NetCreateLoby;
        Player player = NetworkManager.Instance.players[connection.Id];
        player.data.class_type = request.class_type;

        Matchmaking.CreateLoby(player);
    }
    private void OnJoinLobyRequest(NetMessage message, Connection connection)
    {
        NetJoinLoby request = message as NetJoinLoby;
        Player player = NetworkManager.Instance.players[connection.Id];

        Matchmaking.JoinLoby(player, request.loby_id);
    }
}