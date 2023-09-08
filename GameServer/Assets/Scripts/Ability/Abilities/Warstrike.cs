using Newtonsoft.Json;
using System.Collections.Generic;

public class Warstrike : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    private Unit enemy { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Warstrike() : base() { }
    public Warstrike(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        if(enemy != null)
        {
            Hex _cast_unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(unit);
            Hex _enemy_unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(enemy);
            if (_enemy_unit_hex != null && targetable_hex != null && _cast_unit_hex != null)
            {
                unit.Move(_cast_unit_hex, targetable_hex);

                WarstrikeAction warstrike_action = new WarstrikeAction(unit, new AbilityData()
                {
                    cc = 2, amount = unit.stats.damage
                });;
                ((ITargetableSingleHex)warstrike_action).SetAbility(_enemy_unit_hex);

                unit.AddBehaviourToWork(warstrike_action);
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
                    targetable_hex = hexes[i - 1];
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

public class WarstrikeAction : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public WarstrikeAction() : base() { }
    public WarstrikeAction(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }

    public override void Execute()
    {
        Unit enemy = targetable_hex.GetUnit();
        if(enemy != null)
        {
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
            if (!enemy.IsDead())
                enemy.ccs.Add(new Stun(unit, enemy, ability_data.cc));
        }
        Exit();
    }
}
