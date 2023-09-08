using Newtonsoft.Json;
using Riptide;
using Riptide.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
    public int connection_id { get; set; }
    public string device_id { get; set; }
    public long match_id { get; set; }
    public PlayerData player_data { get; set; }
    public Player(int _id)
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
    READY = 1
}

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
    [SerializeField] private ushort max_clinet_count;
    public bool gameobject_visibility = false;
    [SerializeField] private float update_matches_state_interval;
    private Coroutine poll_update_match_stats_corutine;
    [SerializeField] private float keep_alive_interval;
    private Coroutine keep_alive_clients_corutine;

    public Dictionary<ushort,Player> players = new Dictionary<ushort,Player>();

    public Dictionary<long,Game> games = new Dictionary<long, Game>();
    Queue<Game> games_to_add = new Queue<Game>();
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
         RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
         Server = new Server();
         Server.MessageReceived += OnRecievedMessage;
         Server.ClientConnected += OnClientConnected;
         Server.ClientDisconnected += OnClientDisconnected;
         Server.Start(port, max_clinet_count);

         Reciever = new Receiver();
         Reciever.Subscibe();

         //poll_update_match_stats_corutine = StartCoroutine(UpdateMatchesCoroutine());
         keep_alive_clients_corutine = StartCoroutine(KeepAliveClients());
         UpdateMatchesState();
    }
    private void Update()
    {
        foreach (Game game in games.Values)
            game.Update();

        if (games_to_add.Count > 0)
        {
            while (games_to_add.Count != 0)
            {
                Game game_to_add = games_to_add.Dequeue();
                games.Add(game_to_add.match_id, game_to_add);
                game_to_add.Init();

                //string json = NetworkManager.Serialize(game_to_add);
                //File.WriteAllText("ChallengeRoyaleGame.json", json);
            }
        }

    }
    private void FixedUpdate()
    {
        Server.Update();
    }

    private void OnApplicationQuit()
    {
        StopCoroutine(keep_alive_clients_corutine);
        StopCoroutine(poll_update_match_stats_corutine);
        Server.Stop();
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
            if(games.TryGetValue(player.match_id, out Game game))
                game.players.Remove(player);

            players.Remove(connection.Id);
        }
        Server.DisconnectClient(connection);
    }
    private IEnumerator KeepAliveClients()
    {
        while (true)
        {
            yield return new WaitForSeconds(keep_alive_interval);
            Sender.SendToAllClients_Reliable(new NetKeepAlive());
        }
    }
    private IEnumerator UpdateMatchesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(update_matches_state_interval);
            UpdateMatchesState();
        }
    }
    private async void UpdateMatchesState()
    {
        List<int> matches_ids = await Database.UpdateMatchesState();

        if (matches_ids != null)
            foreach (int _match_id in matches_ids)
            {
                Map map = new ChallengeRoyaleMap(4, 4, 1.05f, 1);
                Game game = new ChallengeRoyaleGame(_match_id, map, 10);
                games_to_add.Enqueue(game);
            }
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

        if (msg != null)
            msg.ReceivedOnServer(e.FromConnection);
    }


    #region NetMessages Events
    public static Action<NetMessage, Connection> S_ON_AUTH_REQUEST;
    public static Action<NetMessage, Connection> S_ON_SYNC_REQUEST;
    public static Action<NetMessage, Connection> S_ON_ATTACK_REQUEST;
    public static Action<NetMessage, Connection> S_ON_MOVE_REQUEST;
    public static Action<NetMessage, Connection> S_ON_SINGLE_TARGET_ABILITY_REQUEST;
    public static Action<NetMessage, Connection> S_ON_INSTANT_ABILITY_REQUEST;
    public static Action<NetMessage, Connection> S_ON_MULTIPLE_TARGETS_ABILITY_REQUEST;
    public static Action<NetMessage, Connection> S_ON_UPGRADE_CLASS_REQUEST;
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
