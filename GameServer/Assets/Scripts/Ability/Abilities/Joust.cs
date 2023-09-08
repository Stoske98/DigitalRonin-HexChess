using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Joust : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    private Unit enemy { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Joust() : base() { }
    public Joust(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        Hex _cast_unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(unit);
        if (enemy != null && _cast_unit_hex != null)
        {
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

            if(_cast_unit_hex != null)
            {
                unit.Move(_cast_unit_hex, targetable_hex);

                if (!enemy.IsDead())
                {
                    KnightMovement knigh_movement = new KnightMovement(enemy, 2);
                    knigh_movement.SetPath(targetable_hex, _cast_unit_hex);
                    enemy.AddBehaviourToWork(knigh_movement);

                }
            }
        }
        enemy = null;
        targetable_hex = null;
        Exit();
    }
    public void SetAbility(Hex _targetable_hex)
    {
        enemy = TryToGetEnemyUnit(_targetable_hex);
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        Game game = NetworkManager.Instance.games[unit.match_id];

        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UP, _unit_hex, ability_data.range)));
        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.DOWN, _unit_hex, ability_data.range)));
        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_LEFT, _unit_hex, ability_data.range)));
        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_LEFT, _unit_hex, ability_data.range)));
        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_RIGHT, _unit_hex, ability_data.range)));
        _available_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_RIGHT, _unit_hex, ability_data.range)));

        return _available_moves;
    }

    private List<Hex> AvailableMovesInDirection(List<Hex> hexes)
    {
        int count = 0;
        for (int i = 0; i < hexes.Count; i++)
        {
            Unit enemy = hexes[i].GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
            {
                count = i;
                break;
            }

            if (i == hexes.Count - 1)
            {
                hexes.Clear();
                return hexes;
            }
        }
        for (int i = hexes.Count - 1; i > count; i--)
            hexes.RemoveAt(i);

        if (hexes.Count == 1)
            hexes.Clear();

        return hexes;
    }

    private Unit TryToGetEnemyUnit(Hex _target_hex)
    {
        Game game = NetworkManager.Instance.games[unit.match_id];
        Hex _cast_unit_hex = game.map.GetHex(unit);
        if (_cast_unit_hex != null)
        {
            Unit enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.UP, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.DOWN, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
        }

        return null;
    }

    private Unit CheckIsEnemyOnDirection(Hex _target_hex, List<Hex> hexes)
    {
        if (hexes.Contains(_target_hex))
            for (int i = 0; i < hexes.Count; i++)
            {
                Unit enemy = hexes[i].GetUnit();
                if (enemy != null && enemy.class_type != unit.class_type)
                {
                    targetable_hex = hexes[i];
                    return enemy;
                }

            }

        return null;
    }

    public void Upgrade()
    {
        ability_data.range += 1;
        ability_data.max_cooldown += 1;
    }
}
