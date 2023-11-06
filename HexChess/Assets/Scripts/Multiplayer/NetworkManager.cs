using Newtonsoft.Json;
using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player
{
    public string device_id { get; set; }
    public int match_id { get; set; }
    public PlayerData data { get; set; }
}
public class PlayerData
{
    public string nickname { get; set; }
    public int rank { get; set; }
    public ClassType class_type { get; set; }
}
public class NetworkManager : MonoBehaviour
{
    #region NetworkManager Singleton
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
    public Client Client { get; private set; }
    public Receiver Reciever { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    public Player player;
    [Header("PLAYER SETTINGS")]
    public bool Test;
    public string test_device_id;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, null, Debug.LogError, false);
        Client = new Client();
        Client.MessageReceived += OnRecievedMessage;

        /*Client.Connected += OnClientConnected;
        Client.ConnectionFailed += OnClientFailedConnection;
        Client.Disconnected += OnClientDisconnected;*/
        Reciever = new Receiver();
        Reciever.SubscribeMainMenu();
        Client.Connect($"{ip}:{port}");
        
        player = new Player();
        if (Test)
        {
            player.device_id = test_device_id;
            player.match_id = 2;

        }
        else
            player.device_id = SystemInfo.deviceUniqueIdentifier;
       
    }
    private void FixedUpdate()
    {
        Client.Update();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }
    private void OnRecievedMessage(object sender, MessageReceivedEventArgs e)
    {
        NetMessage msg = null;
        OpCode op_code = (OpCode)e.MessageId;
        switch (op_code)
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
            case OpCode.ON_GAME_EXIST:
                msg = new NetGameExist(e.Message);
                break;
            case OpCode.ON_LOGIN:
                msg = new NetLogin(e.Message);
                break;
            case OpCode.ON_SYNC:
                msg = new NetSync(e.Message);
                break;
            case OpCode.ON_ATTACK:
                msg = new NetAttack(e.Message);
                break;
            case OpCode.ON_MOVE:
                msg = new NetMove(e.Message);
                break;
            case OpCode.ON_SINGLE_TARGET_ABILITY:
                msg = new NetSingleTargetAbilility(e.Message);
                break;
            case OpCode.ON_MULTIPLE_TARGETS_ABILITY:
                msg = new NetMultipeTargetsAbilility(e.Message);
                break;
            case OpCode.ON_INSTANT_ABILITY:
                msg = new NetInstantAbility(e.Message);
                break;
            case OpCode.ON_END_TURN:
                msg = new NetEndTurn(e.Message);
                break;
            case OpCode.ON_UPGRADE_CLASS:
                msg = new NetUpgradeClass(e.Message);
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
            case OpCode.ON_RECONNECT:
                msg = new NetReconnect(e.Message);
                break;
            case OpCode.ON_DISCONNECT:
                msg = new NetDisconnect(e.Message);
                break;
            case OpCode.ON_END_GAME:
                msg = new NetEndGame(e.Message);
                break;
            default:
                break;
        }
        msg.ReceivedOnClient();
    }
   
    #region NetMessages Events
    public static Action<NetMessage> C_ON_KEEP_ALIVE_RESPONESS;
    public static Action<NetMessage> C_ON_WELCOME_RESPONESS;
    public static Action<NetMessage> C_ON_AUTH_RESPONESS;
    public static Action<NetMessage> C_ON_GAME_EXIST_RESPONESS;
    public static Action<NetMessage> C_ON_LOGIN_RESPONESS;
    public static Action<NetMessage> C_ON_SYNC_RESPONESS;
    public static Action<NetMessage> C_ON_MOVE_RESPONESS;
    public static Action<NetMessage> C_ON_ATTACK_RESPONESS;
    public static Action<NetMessage> C_ON_SINGLE_TARGET_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_INSTANT_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_END_TURN_RESPONESS;
    public static Action<NetMessage> C_ON_UPGRADE_CLASS_RESPONESS;
    public static Action<NetMessage> C_ON_CHANGE_NICKNAME_RESPONESS;
    public static Action<NetMessage> C_ON_CREATE_TICKET_RESPONESS;
    public static Action<NetMessage> C_ON_FIND_MATCH_RESPONESS;
    public static Action<NetMessage> C_ON_STOP_MATCH_FINDING_RESPONESS;
    public static Action<NetMessage> C_ON_ACCEPT_MATCH_RESPONESS;
    public static Action<NetMessage> C_ON_DECLINE_MATCH_RESPONESS;
    public static Action<NetMessage> C_ON_MATCH_CREATED_RESPONESS;
    public static Action<NetMessage> C_ON_CREATE_LOBY_RESPONESS;
    public static Action<NetMessage> C_ON_JOIN_LOBY_RESPONESS;
    public static Action<NetMessage> C_ON_DISCONNECT_RESPONESS;
    public static Action<NetMessage> C_ON_RECONNECT_RESPONESS;
    public static Action<NetMessage> C_ON_END_GAME_RESPONESS;
    #endregion

    public static string Serialize<T>(T obj)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            Converters = new List<JsonConverter>
            {
                new CustomConverters.Vector3Converter(),
                new CustomConverters.QuaternionConverter(),
                new CustomConverters.Vector2IntConverter(),
            }
        };
        return JsonConvert.SerializeObject(obj, settings);
    }

    public static T Deserialize<T>(string data)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            Converters = new List<JsonConverter>
            {
                new CustomConverters.Vector3Converter(),
                new CustomConverters.QuaternionConverter(),
                new CustomConverters.Vector2IntConverter(),
            }

        };
        return JsonConvert.DeserializeObject<T>(data, settings);
    }
}
