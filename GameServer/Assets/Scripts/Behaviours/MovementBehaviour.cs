using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
public abstract class MovementBehaviour : Behaviour
{
    protected float movement_speed = 3f;
    protected float rotation_speed = 7.5f;
    protected List<Hex> path { get; set; }
    [JsonRequired] protected int range { get; set; }
    public MovementBehaviour() : base() { path = new List<Hex>(); }
    public MovementBehaviour(Unit _unit)
    {
        unit = _unit;
        path = new List<Hex>();
    }
    public override void Execute()
    {
        if (path.Count != 0)
        {
            if ((path[0].game_object.transform.position - unit.game_object.transform.position).magnitude > 0.1f)
            {
                unit.game_object.transform.position +=
                    (path[0].game_object.transform.position - unit.game_object.transform.position).normalized * movement_speed * Time.deltaTime;

                unit.target_rotation = Quaternion.LookRotation(path[0].game_object.transform.position - unit.game_object.transform.position, Vector3.up);
                unit.game_object.transform.rotation = Quaternion.Slerp(unit.game_object.transform.rotation, unit.target_rotation, Time.deltaTime * rotation_speed);
            }
            else
            {

                if (path.Count == 1)
                {
                    path[0].PlaceObject(unit);
                    NetworkManager.Instance.games[unit.match_id].game_events.OnEndMovement_Global?.Invoke(path[0]);
                }

                path[0].TriggerModifier(unit, path[0]);
                path.RemoveAt(0);
            }
        }
        else
            Exit();
    }
    public override void Enter()
    {
    }
    public override void Exit()
    {
        base.Exit();
    }
    public List<Hex> GetPath()
    {
        return path;
    }
    public abstract List<Hex> GetAvailableMoves(Hex _unit_hex);

    public virtual void SetPath(Hex _unit_hex, Hex _desired_hex)
    {
        _unit_hex.RemoveUnit();
        time = Time.time;
    }
}
