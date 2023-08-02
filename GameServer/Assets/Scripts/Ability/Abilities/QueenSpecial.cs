using System.Collections.Generic;

public class QueenSpecial : TargetableAbility
{
    private Unit enemy;
    public QueenSpecial() : base() { }
    public QueenSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {
        Hex _cast_unit_hex = NetworkManager.Instance.games[unit.match_id].GetHex(unit);
        if (targetable_hex != null && _cast_unit_hex != null)
        {
            unit.Move(_cast_unit_hex, targetable_hex);
            unit.Attack(enemy);
        }
        else 
            unit.Attack(enemy);

        targetable_hex = null;
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = NetworkManager.Instance.games[unit.match_id];

        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.UP, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.DOWN, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.LOWER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.UPPER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.LOWER_RIGHT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetHexesInDirection(Direction.UPPER_RIGHT, _unit_hex,ability_data.range)));

        return _ability_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        enemy = TryToGetEnemyUnit(_targetable_hex);
    }
    private List<Hex> AvailableMovesInDirection(Hex center, List<Hex> hexes)
    {
        int count = 0;
        for (int i = 0; i < hexes.Count; i++)
        {
            if (!hexes[i].IsWalkable() && hexes[i].GetUnit().class_type != center.GetUnit().class_type)
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
    private Unit TryToGetEnemyUnit(Hex _target_hex)
    {
        Game game = NetworkManager.Instance.games[unit.match_id];
        Hex _cast_unit_hex = game.GetHex(unit);
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
                if (!hexes[i].IsWalkable() && hexes[i].GetUnit().class_type != unit.class_type)
                {
                    if(i > 0)
                        targetable_hex = hexes[i - 1];

                    return hexes[i].GetUnit();
                }

        return null;
    }
}
