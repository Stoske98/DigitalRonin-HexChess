using System.Collections.Generic;

public class NoAttack : AttackBehaviour
{
    public NoAttack() : base() { }
    public NoAttack(Unit _unit) : base(_unit) { }
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }

    public override void SetAttack(Unit _target)
    {
        throw new System.NotImplementedException();
    }
}

