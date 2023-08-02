using System.Collections.Generic;

public class Necromancy : TargetableAbility
{
    public Necromancy() : base() { }
    public Necromancy(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        if(targetable_hex.GetUnit().class_type == unit.class_type)
        {
            Heal(targetable_hex.GetUnit());
            unit.RecieveDamage(new MagicDamage(unit, ability_data.amount));
        }else
        {
            Heal(unit);
            targetable_hex.GetUnit().RecieveDamage(new MagicDamage(unit, ability_data.amount));
        }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable())
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }
    private void Heal(Unit unit)
    {
        if (unit.stats.current_health + ability_data.amount > unit.stats.max_health)
            unit.stats.current_health = unit.stats.max_health;
        else
            unit.stats.current_health += ability_data.amount;
    }
    
}