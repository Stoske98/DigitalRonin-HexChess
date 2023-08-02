using System.Collections.Generic;

public class Skyfall : TargetableAbility
{
    public Skyfall() : base() { }
    public Skyfall(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        targetable_hex.GetUnit().RecieveDamage(new MagicDamage(unit, ability_data.amount));
        if(!targetable_hex.IsWalkable())
            targetable_hex.GetUnit().ccs.Add(new Stun(2));

        foreach (Hex hex in targetable_hex.neighbors)
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                hex.GetUnit().RecieveDamage(new MagicDamage(unit, ability_data.amount));

        Exit();

    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }
}