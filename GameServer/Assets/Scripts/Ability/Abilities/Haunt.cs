using Newtonsoft.Json;
using System.Collections.Generic;

public class Haunt : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    private Unit enemy { get; set; }
    private Unit queen_soul { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    [JsonIgnore] public Hex soul_start_hex { get; set; }
    public Haunt() : base() { }
    public Haunt(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        Game game = NetworkManager.Instance.games[unit.match_id];

        queen_soul = Spawner.SpawnUnit(game, unit.unit_type, unit.class_type, soul_start_hex);

        queen_soul.stats.damage = unit.stats.damage;
        if (enemy != null && targetable_hex != null && soul_start_hex != null)
        {
            GhostMovement ghost_movement_1 = new GhostMovement(queen_soul);
            ghost_movement_1.SetPath(soul_start_hex, targetable_hex);
            queen_soul.AddBehaviourToWork(ghost_movement_1);

            Attack _attack = new Attack(queen_soul);
            _attack.SetAttack(enemy);
            queen_soul.AddBehaviourToWork(_attack);

            GhostMovement ghost_movement_2 = new GhostMovement(queen_soul);
            ghost_movement_2.SetPath(targetable_hex, soul_start_hex);
            queen_soul.AddBehaviourToWork(ghost_movement_2);
            ghost_movement_2.OnEndBehaviour += OnEndMovementQueenSoul;

        }

        soul_start_hex = null;
        targetable_hex = null;
        Exit();
    }

    private void OnEndMovementQueenSoul(Behaviour behaviour)
    {
        //dont remove soul just place far away and use agian
        behaviour.OnEndBehaviour -= OnEndMovementQueenSoul;
        Game game = NetworkManager.Instance.games[unit.match_id];

        game.map.GetHex(queen_soul)?.RemoveObject(queen_soul);
        game.object_manager.RemoveObject(queen_soul);
        queen_soul = null;
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
                    soul_start_hex = hexes[0];
                    targetable_hex = hexes[i - 1];
                    return enemy;
                }

            }

        return null;
    }

    public void Upgrade()
    {
        ability_data.range += 10;
    }
}
