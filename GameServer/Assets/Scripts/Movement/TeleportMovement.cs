using System.Collections.Generic;
using UnityEngine;

public class TeleportMovement : MovementBehaviour
{
    public TeleportMovement() : base() { }
    public TeleportMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public TeleportMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override void Execute()
    {
        if(Time.time >= time + 1.5f)
        {
            unit.game_object.transform.LookAt(path[1].game_object.transform);
            unit.game_object.transform.position = path[1].game_object.transform.position;

            path[1].PlaceObject(unit);
            NetworkManager.Instance.games[unit.match_id].game_events.OnEndMovement_Global?.Invoke(path[1]);

            path[1].TriggerModifier(unit, path[1]);

            path.Clear();

            Exit();
        }
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].HexesInRange(_unit_hex, range))
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
}



