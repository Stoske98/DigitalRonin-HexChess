using Newtonsoft.Json;
using System.Collections.Generic;

public class QueenSpecial : TargetableAbility, ITargetableSingleHex, IUpgradable, ISubscribe
{
    private Unit enemy { get; set; }
    [JsonRequired] private bool upgraded { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public QueenSpecial() : base() { }
    public QueenSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { upgraded = false; }

    public override void Execute()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
        if (enemy != null && targetable_hex != null && _cast_unit_hex != null)
        {
            unit.Move(_cast_unit_hex, targetable_hex);
            unit.Attack(enemy);
        }

        enemy = null;
        targetable_hex = null;
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = GameManager.Instance.game;

        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UP, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.DOWN, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_RIGHT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_RIGHT, _unit_hex, ability_data.range)));

        return _ability_moves;
    }

    public void SetAbility(Hex _targetable_hex)
    {
        enemy = TryToGetEnemyUnit(_targetable_hex);
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
        Game game = GameManager.Instance.game;
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
        if (unit.level == 3)
        {
            upgraded = true;
            ability_data.amount = 25;
            unit.events.OnStartAttack_local += OnStartAttack;
        }
    }

    public void RegisterEvents()
    {
        if (upgraded)
            unit.events.OnStartAttack_local += OnStartAttack;
    }

    public void UnregisterEvents()
    {
        if (upgraded)
            unit.events.OnStartAttack_local -= OnStartAttack;
    }
    private Damage OnStartAttack(Damage damage)
    {
        if (GameManager.Instance.game.random_seeds_generator.PercentCalc(ability_data.amount))
            return new CritDamage(damage.unit, damage.amount, 150);

        return damage;
    }
}
