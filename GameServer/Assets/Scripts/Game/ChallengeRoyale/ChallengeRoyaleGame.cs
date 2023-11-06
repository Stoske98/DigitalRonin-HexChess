using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChallengeRoyaleGame : Game
{
    public bool is_activated { set; get; }
    public ShardController shard_controller { get; set; }
    [JsonConverter(typeof(CustomConverters.OuterGameObjectConverter))] public List<GameObject> outer_hexes_obj { get; set; }

    [JsonRequired] private int counter = 0;
    [JsonRequired] private int remove_on_turn = 10;
    [JsonRequired] private int n;
    [JsonRequired] private bool is_on_start = false;

    [JsonIgnore]
    public readonly List<Vector2Int> exp_flag_coordinates = new List<Vector2Int> { new Vector2Int(-2, 1), new Vector2Int(2, -1),
                                                                                   /*new Vector2Int(0, 2), new Vector2Int(0, -2)*/ };
    
    [JsonConstructor] public ChallengeRoyaleGame() : base() { outer_hexes_obj = new List<GameObject>(); }
    public ChallengeRoyaleGame(int _match_id, Map _map, int _remove_on_turn) : base(_match_id, _map)
    {
        remove_on_turn = _remove_on_turn;
        n = map.GetN();

        shard_controller = new ShardController();
        shard_controller.InitControllers();

        foreach (var flag_coordinate in exp_flag_coordinates)
            Spawner.SpawnFlag(this, map.GetHex(flag_coordinate.x, flag_coordinate.y));

        outer_hexes_obj = new List<GameObject>();
        is_on_start = false;
    }
    public override void Init()
    {
        map.SpawnUnits(this);
        object_manager.ProcessPendingActions();

    }
    public void Activate()
    {
        if (!is_activated)
        {
            is_activated = true;
            game_events.OnEndPlayerTurn_Global += Countdown;
        }
    }
    public void Countdown()
    {
        if (!is_on_start)
        {
            is_on_start = true;
        }
        else { counter++; }

        if (ShouldRemoveOuterFields() && n > 1)
            RemoveOuterFields();

        if (n == 1)
            game_events.OnEndPlayerTurn_Global -= Countdown;

    }

    private bool ShouldRemoveOuterFields()
    {
        if (counter == remove_on_turn)
        {
            counter = 0;
            return true;
        }

        return false;
    }

    public void RemoveOuterFields()
    {
        Hex center_hex = map.GetHex(0, 0);
        List<Hex> inner_hexes = map.HexesInRange(center_hex, n - 1);
        n--;

        Material material = MapContainer.Instance.outer_field_material;
        List<Hex> outer_hexes = new List<Hex>();
        foreach (var hex in map.hexes)
        {
            if (!inner_hexes.Contains(hex))
            {
                Unit unit = hex.GetUnit();
                if (unit != null)
                    unit.Die(hex);

                foreach (var obj in hex.objects)
                    object_manager.RemoveObject(obj);

                hex.SetMaterial(material);
                hex.objects.Clear();
                outer_hexes.Add(hex);
            }
        }

        foreach (Hex hex in outer_hexes)
        {
            hex.game_object.name = "OUTTER HEX";
            hex.game_object.GetComponent<MeshRenderer>().material = material;
            outer_hexes_obj.Add(hex.game_object);
            map.hexes.Remove(hex);
        }
    }

    public override void EndTurn()
    {
        move++;
        class_on_turn = class_on_turn == ClassType.Light ? ClassType.Dark : ClassType.Light;

        UpdateFlagExp();

        foreach (var obj in object_manager.objects)
        {
            if(obj is Unit unit && unit.class_type == class_on_turn)
            {
                unit.UpdateAbilitiesCooldown();
                unit.UpdateCCsCooldown();
            }
        }

        game_events.OnEndPlayerTurn_Global?.Invoke();
        action_done = false;

        SendTurnMessageWithDelayAsync();
    }
    private async void SendTurnMessageWithDelayAsync()
    {
        await Task.Delay(10); 

        NetEndTurn responess = new NetEndTurn();
        responess.class_on_turn = class_on_turn;

        SendMessageToPlayers(responess);
    }

    private void UpdateFlagExp()
    {
        foreach (Vector2Int coordiante in exp_flag_coordinates)
        {
            Hex hex = map.GetHex(coordiante.x, coordiante.y);
            if(hex != null)
            {
                Flag flag = null;
                foreach (IObject obj in hex.objects)
                    if(obj is Flag _flag)
                        flag = _flag;

                Unit unit = hex?.GetUnit();


                if (unit != null && flag.class_type != unit.class_type && class_on_turn == unit.class_type)
                {
                    flag.class_type = unit.class_type;
                    continue;
                }
                else if (unit != null && flag.class_type == unit.class_type && class_on_turn != unit.class_type)
                {
                    flag.OccupiedField();
                    continue;
                }

                if (flag.is_ocuppied && flag.class_type == class_on_turn)
                {
                    if (class_on_turn == ClassType.Light)
                        shard_controller.light_shards++;
                    else if (class_on_turn == ClassType.Dark)
                        shard_controller.dark_shards++;
                }

            }
        }
    }

    public override void RegisterEvents()
    {
        if (is_activated)
            game_events.OnEndPlayerTurn_Global += Countdown;
    }

    public override void UnregisterEvents()
    {
        if (is_activated)
            game_events.OnEndPlayerTurn_Global -= Countdown;
    }
}
public class Flag : IObject
{
    public string id { get; set; }
    public string game_object_path { get; set; }
    [JsonConverter(typeof(CustomConverters.GameObjectConverter))] public GameObject game_object { get; set; }
    public Visibility visibility { get; set; }
    public ClassType class_type { get; set; }
    public bool is_ocuppied { get; set; }

    public Flag() { }
    public Flag(Game _game, Hex _hex, string _game_object_path)
    {
        id = _game.random_seeds_generator.GetRandomIdsSeed();
        class_type = ClassType.None;
        visibility = Visibility.BOTH;
        game_object_path = _game_object_path;

        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(game_object_path));
        game_object.transform.SetParent(_hex.game_object.transform);
        game_object.name = "Flag: ["+ _hex .coordinates.x+ "]["+ _hex .coordinates.y+ "]";
    }

    public void OccupiedField()
    {
        if(class_type == ClassType.Light)
            game_object.GetComponentInChildren<Renderer>().material.color = Color.blue;
        else if(class_type == ClassType.Dark)
            game_object.GetComponentInChildren<Renderer>().material.color = Color.red;

        is_ocuppied = true;
    }
}


