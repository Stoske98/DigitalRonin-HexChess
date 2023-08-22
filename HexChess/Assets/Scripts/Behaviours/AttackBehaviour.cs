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
        time = UnityEngine.Time.time;

        unit.animator?.SetBool("Attack", true);

    }
    public override void Exit()
    {
        base.Exit();
        unit.animator?.SetBool("Attack", false);
    }
    public virtual void SetAttack(Unit _target)
    {
        target = _target;
        damage = new PhysicalDamage(unit, unit.stats.damage);
    }
    public virtual List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        List<Hex> _attack_moves = new List<Hex>();

        List<Hex> _hexes = GameManager.Instance.game.map.HexesInRange(_unit_hex, unit.stats.attack_range);

        foreach (var _hex_in_range in _hexes)
            if (!_hex_in_range.IsWalkable() && _hex_in_range.GetUnit().class_type != _unit_hex.GetUnit().class_type)
                _attack_moves.Add(_hex_in_range);

        unit.events.OnGetAttackMoves_Local?.Invoke(_unit_hex, _attack_moves);

        return _attack_moves;
    }

}

