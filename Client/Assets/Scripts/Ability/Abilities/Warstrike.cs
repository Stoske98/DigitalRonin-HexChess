using System.Collections.Generic;

public class Warstrike : TargetableAbility
{
    Unit enemy;
    public Warstrike() : base() { }
    public Warstrike(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        if(enemy != null)
        {
            enemy.RecieveDamage(new MagicDamage(unit, ability_data.amount));
            if (!enemy.IsDeath())
                enemy.ccs.Add(new Stun(2));

        }
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
        enemy = targetable_hex.GetUnit();
    }
}
