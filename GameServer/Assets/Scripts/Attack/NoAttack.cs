using System.Collections.Generic;

public class NoAttack : AttackBehaviour
{
    public NoAttack() : base() { }
    public NoAttack(Unit _unit) : base(_unit) { }
    public override void Execute()
    {
        Exit();
    }

    public override List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }

}

