using Newtonsoft.Json;
using System.Collections.Generic;

public class QueensCommand : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    private Unit enemy { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public QueensCommand() : base() { }
    public QueensCommand(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {
        Hex enemy_hex = GameManager.Instance.game.map.GetHex(enemy);
        if (enemy != null && enemy_hex != null)
        {
            enemy.Move(enemy_hex, targetable_hex);
            enemy.events.OnEndMovement_Local += OnEnemyEndMovement;

        }
        targetable_hex = null;
        Exit();
    }
    private void OnEnemyEndMovement(Hex hex)
    {
        AttackBehaviour attack_behaviour = unit.GetBehaviour<AttackBehaviour>();
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
        Hex _enemy_unit_hex = GameManager.Instance.game.map.GetHex(enemy);
        if (enemy != null && attack_behaviour != null && _cast_unit_hex != null && _enemy_unit_hex != null && attack_behaviour.GetAttackMoves(_cast_unit_hex).Contains(_enemy_unit_hex))
            unit.Attack(enemy);

        enemy.events.OnEndMovement_Local -= OnEnemyEndMovement;
        enemy = null;
    }
    public void Upgrade()
    {
        ability_data.range = 10;
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
            if (enemy != null)
            {
                if (enemy.class_type != unit.class_type)
                {
                    count = i;
                    break;
                }
                else
                {
                    hexes.Clear();
                    return hexes;
                }
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
                    targetable_hex = hexes[0];
                    return enemy;
                }

            }

        return null;
    }
}
