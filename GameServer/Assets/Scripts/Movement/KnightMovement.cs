using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : MovementBehaviour
{
    public KnightMovement() : base() { }
    public KnightMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public KnightMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, range))
            if (hex.IsWalkable() && (Mathf.Abs(hex.coordinates.x - _unit_hex.coordinates.x) == range
                || Mathf.Abs(hex.coordinates.y - _unit_hex.coordinates.y) == range
                || Mathf.Abs(hex.S - _unit_hex.S) == range))
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
}




