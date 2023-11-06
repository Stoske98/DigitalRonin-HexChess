using Newtonsoft.Json;
using System.Collections.Generic;

public enum ClassType
{
    None = 0,
    Light = 1,
    Dark = 2,
}

public enum UnitType
{
    Swordsman = 0,
    Knight = 1,
    Tank = 2,
    Archer = 3,
    Wizard = 4, 
    Jester = 5,
    Queen = 6,
    King = 7, 
    QueenSoul = 8,
    Stone = 9,

}
public abstract class Game : ISubscribe
{
    [JsonIgnore] public int match_id {get; set;}
    [JsonConverter(typeof(CustomConverters.MapConverter))] public Map map { get; set; }
    public ClassType class_on_turn { get; set; }
    public int move { get; set; }
    [JsonIgnore] public GameEvents game_events { get; set; }
    public ObjectManager object_manager { get; set; }
    public RandomSeedsGenerator random_seeds_generator{ get; set; }
    public int death_light { get; set; }
    public int death_dark { get; set; }
    [JsonIgnore] public List<Player> players { get; set; }

    [JsonIgnore] public bool action_done { get; set; }

    [JsonConstructor]
    public Game() 
    {
        players = new List<Player>();
        game_events = new GameEvents();
        object_manager = new ObjectManager();
        random_seeds_generator = new RandomSeedsGenerator();
        action_done = false;
    }
    public Game(int _match_id, Map _map)
    {
        match_id = _match_id;
        map = _map;
        players = new List<Player>();
        game_events = new GameEvents();
        object_manager = new ObjectManager();
        random_seeds_generator = new RandomSeedsGenerator(1000, 1000);
        death_light = 0;
        death_dark = 0;

        move = 1;
        class_on_turn = ClassType.Light;
        action_done = false;
    }
    public void Update()
    {
        if(action_done)
        {
            object_manager.Update();

            if (!object_manager.IsObjectsWorking())
                EndTurn();
        }
    }
    public abstract void Init();
    public abstract void EndTurn();
    public void SendMessageToPlayers(NetMessage responess)
    {
        foreach (Player player in players)
            Sender.SendToClient_Reliable((ushort)player.connection_id, responess);
    }
    //Remove code below
    public List<Hex> GetAllHexesInDirection(Direction direction, Hex center_hex, bool count_unwalkable_fields = true)
    {
        return map.GetAllHexesInDirection(direction, center_hex, count_unwalkable_fields);
    }
    public List<Hex> GetHexesInDirection(Direction direction, Hex center_hex, int range, bool count_unwalkable_fields = true)
    {
        return map.GetHexesInDirection(direction, center_hex,range, count_unwalkable_fields);
    }

    public abstract void RegisterEvents();
    public abstract void UnregisterEvents();
}


