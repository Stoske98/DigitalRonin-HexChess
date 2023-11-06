using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.IO;
using System.Timers;

public class NetworkManager : MonoBehaviour
{
    #region MetworkManager Singleton
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log("Network Manager instance already exist, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    #endregion

    public Server Server { get; private set; }
    public Receiver Reciever { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] public ushort max_clinet_count;

    public Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

    [SerializeField] private float keep_alive_interval;
    private Timer keep_alive_clients_timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, null, Debug.LogError, false);
        Server = new Server();
        Server.MessageReceived += OnRecievedMessage;
        Server.ClientConnected += OnClientConnected;
        Server.ClientDisconnected += OnClientDisconnected;
        Server.Start(port, max_clinet_count);
        

        Reciever = new Receiver();
        Reciever.Subscibe();

        Matchmaking.Initialization();
        keep_alive_clients_timer = new Timer(keep_alive_interval * 1000);
        keep_alive_clients_timer.Elapsed += KeepAliveClients;
        keep_alive_clients_timer.AutoReset = true;
        keep_alive_clients_timer.Start();
    }
    private void FixedUpdate()
    {
        Server.Update();
    }

    private void OnApplicationQuit()
    {
        keep_alive_clients_timer.Stop();
        keep_alive_clients_timer.Dispose();
        Server.Stop();
    }

    private void KeepAliveClients(object sender, ElapsedEventArgs e)
    {
        Sender.SendToAllClients_Reliable(new NetKeepAlive());
    }
    private void OnClientConnected(object sender, ServerConnectedEventArgs e)
    {
        players.Add(e.Client.Id, new Player(e.Client.Id));

        Sender.SendToClient_Reliable(e.Client.Id, new NetWelcome());
    }

    private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        players.Remove(e.Client.Id);
    }

    public void DisconnectPlayer(Connection connection)
    {
        if (players.TryGetValue(connection.Id, out Player player))
        {
            Matchmaking.DeleteTicket(player);
            Matchmaking.DeclineMatch(player);
            players.Remove(connection.Id);
        }

        Server.DisconnectClient(connection);
    }

    private static void OnRecievedMessage(object sender, MessageReceivedEventArgs e)
    {
        NetMessage msg = null;
        OpCode opCode = (OpCode)e.MessageId;

        switch (opCode)
        {
            case OpCode.ON_KEEP_ALIVE:
                msg = new NetKeepAlive(e.Message);
                break;
            case OpCode.ON_WELCOME:
                msg = new NetWelcome(e.Message);
                break;
            case OpCode.ON_AUTH:
                msg = new NetAuthentication(e.Message);
                break;
            case OpCode.ON_CHANGE_NICKNAME:
                msg = new NetChangeNickname(e.Message);
                break;
            case OpCode.ON_CREATE_TICKET:
                msg = new NetCreateTicket(e.Message);
                break;
            case OpCode.ON_FIND_MATCH:
                msg = new NetFindMatch(e.Message);
                break;
            case OpCode.ON_STOP_MATCH_FINDING:
                msg = new NetStopMatchFinding(e.Message);
                break;
            case OpCode.ON_ACCEPT_MATCH:
                msg = new NetAcceptMatch(e.Message);
                break;
            case OpCode.ON_DECLINE_MATCH:
                msg = new NetDeclineMatch(e.Message);
                break;
            case OpCode.ON_MATCH_CREATED:
                msg = new NetMatchCreated(e.Message);
                break;
            case OpCode.ON_CREATE_LOBY:
                msg = new NetCreateLoby(e.Message);
                break;
            case OpCode.ON_JOIN_LOBY:
                msg = new NetJoinLoby(e.Message);
                break;
            default:
                break;
        }

        if (msg != null)
            msg.ReceivedOnServer(e.FromConnection);
    }


    #region NetMessages Events
    public static Action<NetMessage, Connection> S_ON_KEEP_ALIVE_REQUEST;
    public static Action<NetMessage, Connection> S_ON_AUTH_REQUEST;
    public static Action<NetMessage, Connection> S_ON_SYNC_REQUEST;
    public static Action<NetMessage, Connection> S_ON_CHANGE_NICKNAME_REQUEST;
    public static Action<NetMessage, Connection> S_ON_CREATE_TICKET_REQUEST;
    public static Action<NetMessage, Connection> S_ON_FIND_MATCH_REQUEST;
    public static Action<NetMessage, Connection> S_ON_STOP_MATCH_FINDING_REQUEST;
    public static Action<NetMessage, Connection> S_ON_ACCEPT_MATCH_REQUEST;
    public static Action<NetMessage, Connection> S_ON_DECLINE_MATCH_REQUEST;
    public static Action<NetMessage, Connection> S_ON_CREATE_LOBY_REQUEST;
    public static Action<NetMessage, Connection> S_ON_JOIN_LOBY_REQUEST;
    #endregion

    public static string GetInternalIP(AddressFamily type)
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == type)
            {
                return ip.ToString();
            }
        }
        return "0.0.0.0";
    }
    public static string GetExternalIP()
    {
         try
         {
             var ip = IPAddress.Parse(new WebClient().DownloadString("https://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim());
             return ip.ToString();
         }
         catch (Exception)
         {
             try
             {
                 StreamReader sr = new StreamReader(WebRequest.Create("https://checkip.dyndns.org").GetResponse().GetResponseStream());
                 string[] ipAddress = sr.ReadToEnd().Trim().Split(':')[1].Substring(1).Split('<');
                 return ipAddress[0];
             }
             catch (Exception)
             {
                 return "0.0.0.0";
             }
         }
    }
}

public class Player
{
    public ushort connection_id { get; set; }
    public string device_id { get; set; }
    public long match_id { get; set; }
    public Ticket ticket_reference { get; set; }
    public Match match_reference {  get; set; }
    public PlayerData data { get; set; }
    public Player(ushort _id)
    {
        connection_id = _id;
    }

}
public class PlayerData
{
    public long account_id { get; set; }
    public string nickname { get; set; }
    public int rank { get; set; }
    public ClassType class_type { get; set; }
}
public enum MatchState
{
    NOT_READY = 0,
    READY = 1,
    FINISHED = 2,
}
public enum ClassType
{
    None = 0,
    Light = 1,
    Dark = 2,
}