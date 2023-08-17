using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
public abstract class Game
{
    [JsonConverter(typeof(CustomConverters.MapConverter))] public Map map { get; set; }
    public ClassType class_on_turn { get; set; }
    public int move { get; set; }
    [JsonIgnore] public GameEvents game_events { get; set; }
    public ObjectManager object_manager { get; set; }
    public RandomSeedsGenerator random_seeds_generator { get; set; }

    [JsonConstructor]
    public Game() 
    {
        game_events = new GameEvents();
        object_manager = new ObjectManager();
        random_seeds_generator = new RandomSeedsGenerator();
    }
    public Game(Map _map)
    {
        map = _map;
        game_events = new GameEvents();
        object_manager = new ObjectManager();

        move = 1;
        class_on_turn = ClassType.Light;
    }
    public void Update()
    {
        object_manager.Update();
    }
    /* public void MoveUnit(Hex _unit_hex, Hex _desired_hex)
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
     }*/
    public abstract void Init();
    public abstract void EndTurn();

    //Remove code below
    public List<Hex> GetAllHexesInDirection(Direction direction, Hex center_hex, bool count_unwalkable_fields = true)
    {
        return map.GetAllHexesInDirection(direction, center_hex, count_unwalkable_fields);
    }
    public List<Hex> GetHexesInDirection(Direction direction, Hex center_hex, int range, bool count_unwalkable_fields = true)
    {
        return map.GetHexesInDirection(direction, center_hex,range, count_unwalkable_fields);
    }
}
