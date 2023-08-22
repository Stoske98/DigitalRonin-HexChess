using System.Collections.Generic;

public class NoMovement : MovementBehaviour
{
    public NoMovement() : base() { }
    public NoMovement(Unit _unit) : base(_unit) { }
    public override void Execute()
    {
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }
}



