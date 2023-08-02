using System.Collections.Generic;
using UnityEngine;

public class NormalMovement : MovementBehaviour
{
    public NormalMovement() : base() { }
    public NormalMovement(Unit _unit) : base(_unit)
    {
        range = 1;
    }
    public NormalMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in PathFinding.PathFinder.BFS_HexesMoveRange(_unit_hex, range, GameManager.Instance.game.GetMapHexes()))
            if(hex.IsWalkable())
                _available_moves.Add(hex);

        return _available_moves;
    }
    public override void SetPath(Hex _unit_hex, Hex _desired_hex)
    {
        base.SetPath(_unit_hex, _desired_hex);

        path = PathFinding.PathFinder.FindPath_AStar(_unit_hex, _desired_hex, GameManager.Instance.game.GetMapHexes());
        unit.events.OnStartMovement_Local?.Invoke(_unit_hex, _desired_hex);
    }
}



