﻿using System.Collections.Generic;

public class Powershoot : TargetableAbility
{
    Unit enemy = null;
    public Powershoot() : base() { }
    public Powershoot(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        if (enemy != null)
            enemy.RecieveDamage(new MagicDamage(unit, ability_data.amount));

        enemy = null;
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = GameManager.Instance.game;

        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UP, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.DOWN, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UPPER_LEFT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _unit_hex)));

        return _ability_moves;
    }
    public override void SetAbility(Hex _target_hex)
    {
        enemy = TryToGetEnemyUnit(_target_hex);
    }

    private List<Hex> AvailableMovesInDirection(Hex center, List<Hex> hexes)
    {
        int count = 0;
        for (int i = 0; i < hexes.Count; i++)
        {
            if(!hexes[i].IsWalkable() && hexes[i].GetUnit().class_type != center.GetUnit().class_type)
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

        return hexes;
    }

    private Unit TryToGetEnemyUnit(Hex target_hex)
    {
        Game game = GameManager.Instance.game;
        Hex _cast_unit_hex = game.GetHex(unit);
        if(_cast_unit_hex != null)
        {
            Unit enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.UP, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.DOWN, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
        }       

        return null;
    }

    private Unit CheckIsEnemyOnDirection(Hex target_hex, List<Hex> hexes)
    {
        if (hexes.Contains(target_hex))
            foreach (Hex hex in hexes)
                if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                    return hex.GetUnit();

        return null;
    }
}
