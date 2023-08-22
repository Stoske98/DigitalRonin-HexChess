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
    public PlayerData player_data { get; set; }
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
    private Coroutine poll_sync_corutine;

    public Player player;
    [Header("PLAYER SETTINGS")]
    public bool Test;
    public string test_device_id;
    public int test_game_id;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Client = new Client();
        Client.MessageReceived += OnRecievedMessage;

        /*Client.Connected += OnClientConnected;
        Client.ConnectionFailed += OnClientFailedConnection;
        Client.Disconnected += OnClientDisconnected;*/
        Reciever = new Receiver();
        Reciever.Subscibe();
        Client.Connect($"{ip}:{port}");
        
        player = new Player();
        if (Test)
            player.device_id = test_device_id;
        else
            player.device_id = SystemInfo.deviceUniqueIdentifier;

        player.match_id = test_game_id;
       
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
            default:
                break;
        }
        msg.ReceivedOnClient();
    }
    public void StartSyncGameData()
    {
        poll_sync_corutine = StartCoroutine(TryToSync());
    }

    public void StopSyncGameData()
    {
        StopCoroutine(poll_sync_corutine);
    }
    private IEnumerator TryToSync()
    {
        while (true)
        {
            NetSync request = new NetSync()
            {
                match_id = player.match_id,
            };
            Sender.SendToServer_Reliable(request);

            yield return new WaitForSeconds(5);
        }
    }
    #region NetMessages Events
    public static Action<NetMessage> C_ON_KEEP_ALIVE_RESPONESS;
    public static Action<NetMessage> C_ON_WELCOME_RESPONESS;
    public static Action<NetMessage> C_ON_AUTH_RESPONESS;
    public static Action<NetMessage> C_ON_SYNC_RESPONESS;
    public static Action<NetMessage> C_ON_MOVE_RESPONESS;
    public static Action<NetMessage> C_ON_ATTACK_RESPONESS;
    public static Action<NetMessage> C_ON_SINGLE_TARGET_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_INSTANT_ABILITY_RESPONESS;
    public static Action<NetMessage> C_ON_END_TURN_RESPONESS;
    public static Action<NetMessage> S_ON_UPGRADE_CLASS_RESPONESS;
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
