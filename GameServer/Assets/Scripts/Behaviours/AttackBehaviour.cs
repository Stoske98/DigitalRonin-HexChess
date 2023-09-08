using System.Collections.Generic;

public abstract class AttackBehaviour : Behaviour
{
    protected Unit target { get; set; }
    protected Damage damage { get; set; }
    public AttackBehaviour() : base() { }
    public AttackBehaviour(Unit _unit) { unit = _unit; }
    public override void Enter()
    { 
        base.Enter();
        damage = unit.events.OnStartAttack_local?.Invoke(damage) ?? damage;
        unit.game_object.transform.LookAt(target.game_object.transform);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public virtual void SetAttack(Unit _target)
    {
        target = _target;
        damage = new PhysicalDamage(unit, unit.stats.damage);
    }
    public virtual List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        List<Hex> _attack_moves = new List<Hex>();

        List<Hex> _hexes = NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, unit.stats.attack_range);

        foreach (var _hex_in_range in _hexes)
        {
            Unit enemy = _hex_in_range.GetUnit();
            if (enemy != null && unit.class_type != enemy.class_type)
                _attack_moves.Add(_hex_in_range);
        }

        unit.events.OnGetAttackMoves_Local?.Invoke(_unit_hex, _attack_moves);

        return _attack_moves;
    }


}

