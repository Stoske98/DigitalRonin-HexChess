using System.Collections.Generic;

public class FlyingMovement : MovementBehaviour
{
    public FlyingMovement() : base() { }
    public FlyingMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public FlyingMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, range))
            if (hex.IsWalkable())
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetPath(Hex _unit_hex, Hex _desired_hex)
    {
        base.SetPath(_unit_hex, _desired_hex);

        path.Clear();
        path.Add(_unit_hex);
        path.Add(_desired_hex);

        unit.events.OnStartMovement_Local?.Invoke(_unit_hex, _desired_hex);
    }
    public override void Enter()
    {
        base.Enter();
        unit.animator?.SetBool("Run", true);
    }
    public override void Exit()
    {
        base.Exit();
        unit.animator?.SetBool("Run", false);
    }
}