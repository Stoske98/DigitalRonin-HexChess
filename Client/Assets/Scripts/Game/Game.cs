using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public enum ClassType
{
    NONE = 0,
    LIGHT = 1,
    DARK = 2,
}

public enum UnitType
{
    SWORDSMAN = 0,
    KNIGHT = 1,
    TANK = 2,
    ARCHER = 3,
    WIZARD = 4, 
    JESTER = 5,
    QUEEN = 6,
    KING = 7
}
public class RandomSeedsGenerator
{
    [JsonConverter(typeof(CustomConverters.FloatQueueConverter))] public Queue<float> random_seeds { get; set; }

    public RandomSeedsGenerator()
    {
        random_seeds = new Queue<float>();
    }

    public float GetRandomSeed()
    {
        return random_seeds.Dequeue();
    }
}
public abstract class Game
{
    [JsonConverter(typeof(CustomConverters.MapConverter))] public Map map { get; set; }
    public ClassType class_on_turn { get; set; }
    public int move { get; set; }
    [JsonIgnore] public GameEvents game_events { get; set; }

    [JsonRequired] [JsonConverter(typeof(CustomConverters.ObjectListConverter))] public List<IObject> objects { get; set; }
    [JsonIgnore] public List<Unit> units { get; set; }
    public RandomSeedsGenerator random_seeds_generator { get; set; }

    [JsonConstructor]
    public Game() 
    {
        units = new List<Unit>();
        game_events = new GameEvents();
        objects = new List<IObject>();
        random_seeds_generator = new RandomSeedsGenerator();
    }
    public Game(Map _map)
    {
        map = _map;
        game_events = new GameEvents();
        objects = new List<IObject>();
        units = new List<Unit>();

        map.SpawnUnits(this);
        move = 1;
        class_on_turn = ClassType.LIGHT;
    }
    public void Update()
    {
        foreach (var unit in units)
            unit.Update();
    }
    public void MoveUnit(Hex _unit_hex, Hex _desired_hex)
    {
        Unit unit = _unit_hex.GetUnit();
        unit.Move(_unit_hex,_desired_hex);       
    }
    public void AttackUnit(Unit _attacker, Unit _target)
    {
        _attacker.Attack(_target);
    }
    public void UseUnitAbility(Unit unit, Ability ability, Hex targetable_hex = null)
    {
        unit.UseAbility(ability,targetable_hex);
    }
    public abstract void EndTurn();
    public Hex GetHex(int column, int row)
    {
        return map.GetHex(column, row);
    }
    public Hex GetHex(Unit unit)
    {
        return map.GetHex(unit);
    }
    public List<Hex> GetMapHexes()
    {
        return map.hexes;
    }
    public List<Hex> HexesInRange(Hex hex, int range)
    {
        return map.HexesInRange(hex,range);
    }
    public IObject GetObject(string id)
    {
        foreach (IObject obj in objects)
            if (obj.id == id)
                return obj;
        return null;
    }
    public bool PlaceObject(IObject obj, Hex hex)
    {
        if (map.PlaceObject(obj, hex.coordinates.x, hex.coordinates.y))
            return true;
        return false;
    }

    public List<Hex> GetAllHexesInDirection(Direction direction, Hex center_hex, bool count_unwalkable_fields = true)
    {
        return map.GetAllHexesInDirection(direction, center_hex, count_unwalkable_fields);
    }
    public List<Hex> GetHexesInDirection(Direction direction, Hex center_hex, int range, bool count_unwalkable_fields = true)
    {
        return map.GetHexesInDirection(direction, center_hex,range, count_unwalkable_fields);
    }
}


