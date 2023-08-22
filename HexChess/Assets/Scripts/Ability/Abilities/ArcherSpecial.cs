using System.Collections.Generic;
using System.Linq;

public class ArcherSpecial : PassiveAbility 
{
    private int max_range_increment { get; set; }
    public ArcherSpecial() : base() { max_range_increment = 3; }
    public ArcherSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { max_range_increment = 3; }

    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovement;
        unit.events.OnGetAttackMoves_Local += OnGetAttackMoves;
        unit.events.OnStartAttack_local += OnStartAttack;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovement;
        unit.events.OnGetAttackMoves_Local -= OnGetAttackMoves;
        unit.events.OnStartAttack_local -= OnStartAttack;
    }

    private Damage OnStartAttack(Damage damage)
    {
        ability_data.range = 0;
        return damage;
    }
    private void OnGetAttackMoves(Hex hex, List<Hex> list)
    {
        if (ability_data.range > 0)
        {
            Map map = GameManager.Instance.game.map;

            List<Hex> _new_attack_moves = new List<Hex>();
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.UP, hex, unit.class_type, unit.stats.attack_range + ability_data.range));
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.DOWN, hex, unit.class_type, unit.stats.attack_range + ability_data.range));
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.LOWER_LEFT, hex, unit.class_type, unit.stats.attack_range + ability_data.range));
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.UPPER_LEFT, hex, unit.class_type, unit.stats.attack_range + ability_data.range));
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.LOWER_RIGHT, hex, unit.class_type, unit.stats.attack_range + ability_data.range));
            _new_attack_moves.AddRange(map.GetEnemyHexesInDirection(Direction.UPPER_RIGHT, hex, unit.class_type, unit.stats.attack_range + ability_data.range));

            list.AddRange(_new_attack_moves.Except(list).ToList());
        }
    }
    private void OnStartMovement(Hex _from_hex, Hex _target_hex)
    {
        if (max_range_increment > ability_data.range)
            ability_data.range += 1;     
    }
}

