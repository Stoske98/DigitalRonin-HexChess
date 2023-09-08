using System.Collections.Generic;

public class DirectionMovement : MovementBehaviour
{
    public DirectionMovement() : base() { }
    public DirectionMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public DirectionMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();
        Game game = GameManager.Instance.game;

        _available_moves.AddRange(game.GetHexesInDirection(Direction.UP,_unit_hex,range,false));
        _available_moves.AddRange(game.GetHexesInDirection(Direction.DOWN, _unit_hex, range, false));
        _available_moves.AddRange(game.GetHexesInDirection(Direction.LOWER_RIGHT, _unit_hex, range, false));
        _available_moves.AddRange(game.GetHexesInDirection(Direction.UPPER_RIGHT, _unit_hex, range, false));
        _available_moves.AddRange(game.GetHexesInDirection(Direction.LOWER_LEFT, _unit_hex, range, false));
        _available_moves.AddRange(game.GetHexesInDirection(Direction.UPPER_LEFT, _unit_hex, range, false));

        return _available_moves;
    }

    public override void SetPath(Hex _unit_hex, Hex _desired_hex)
    {
        base.SetPath(_unit_hex, _desired_hex); 
        
        path = PathFinding.PathFinder.FindPath_AStar(_unit_hex, _desired_hex, GameManager.Instance.game.map);
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

