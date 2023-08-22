﻿using Newtonsoft.Json;
using System.Collections.Generic;

public class TrapAbility : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public TrapAbility() : base() { }
    public TrapAbility(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        Trap trap = Spawner.CreateTrap(unit, this);

        GameManager.Instance.game.map.PlaceObject(trap, targetable_hex.coordinates.x, targetable_hex.coordinates.y);
        GameManager.Instance.game.object_manager.AddObject(trap);

        if (unit.class_type == ClassType.Light && unit.class_type == NetworkManager.Instance.player.player_data.class_type )
            IObject.ObjectVisibility(trap, Visibility.LIGHT);
        else
            IObject.ObjectVisibility(trap, Visibility.DARK);

        Exit();

    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            if (hex != null && hex.IsWalkable())
            {
                bool trapFound = false;

                foreach (var obj in hex.objects)
                {
                    if (obj is Trap trap)
                    {
                        trapFound = true;
                        break;
                    }
                }

                if (!trapFound)
                    _available_moves.Add(hex);
            }
        }

        return _available_moves;
    }
}

public class TrapAbilityFinal : TargetableAbility, ITargetMultipleHexes
{
    public int max_hexes { get; set; }
    public bool has_condition { get; set; }
    public List<Hex> targetable_hexes { get; set; }
    public TrapAbilityFinal() : base() 
    {
        targetable_hexes = new List<Hex>();
        max_hexes = 2;
        has_condition = false;
    }
    public TrapAbilityFinal(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { 
        targetable_hexes = new List<Hex>(); 
        max_hexes = 2; 
        has_condition = false; }
    public override void Execute()
    {
        foreach (var targetable_hex in targetable_hexes)
        {
            Trap trap = Spawner.CreateTrap(unit, this);

            GameManager.Instance.game.map.PlaceObject(trap, targetable_hex.coordinates.x, targetable_hex.coordinates.y);
            GameManager.Instance.game.object_manager.AddObject(trap);

            if (unit.class_type == ClassType.Light && unit.class_type == NetworkManager.Instance.player.player_data.class_type)
                IObject.ObjectVisibility(trap, Visibility.LIGHT);
            else
                IObject.ObjectVisibility(trap, Visibility.DARK);
        }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            if (hex != null && hex.IsWalkable())
            {
                bool trapFound = false;

                foreach (var obj in hex.objects)
                {
                    if (obj is Trap trap)
                    {
                        trapFound = true;
                        break;
                    }
                }

                if (!trapFound)
                    _available_moves.Add(hex);
            }
        }

        return _available_moves;
    }
}