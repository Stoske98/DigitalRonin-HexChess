﻿using Newtonsoft.Json;
using System.Collections.Generic;

public class Skyfall : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Skyfall() : base() { }
    public Skyfall(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        Unit enemy = targetable_hex.GetUnit();
        enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
        if (!enemy.IsDead())
            enemy.ccs.Add(new Stun(ability_data.cc));

        foreach (Hex hex in targetable_hex.GetNeighbors(GameManager.Instance.game.map))
        {
            Unit _unit = hex.GetUnit();
            if (_unit != null && _unit.class_type != unit.class_type)
                _unit.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
        }

    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
}