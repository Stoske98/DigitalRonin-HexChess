using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MovementBehaviour
{
    public GhostMovement() : base() { }
    public GhostMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public GhostMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override void Execute()
    {
        if (path.Count == 0)
        {
            Exit();
            return;
        }

        if (next_hex == null)
            next_hex = path[1];

        if ((next_hex.game_object.transform.position - unit.game_object.transform.position).sqrMagnitude <= 0.1f * 0.1f)
        {
            current_hex.RemoveObject(unit);
            next_hex.PlaceObject(unit);

            path.RemoveAt(0);

            if (path.Count != 0)
            {
                current_hex = next_hex;
                next_hex = path[0];
            }
        }
        else
        {
            unit.game_object.transform.position +=
                (next_hex.game_object.transform.position - unit.game_object.transform.position).normalized * movement_speed * Time.deltaTime;

            unit.target_rotation = Quaternion.LookRotation(next_hex.game_object.transform.position - unit.game_object.transform.position, Vector3.up);
            unit.game_object.transform.rotation = Quaternion.Slerp(unit.game_object.transform.rotation, unit.target_rotation, Time.deltaTime * rotation_speed);
        }
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
    }
}