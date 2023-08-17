using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
public abstract class MovementBehaviour : Behaviour
{
    protected float movement_speed = 3f;
    protected float rotation_speed = 7.5f;
    protected List<Hex> path { get; set; }
    protected Hex current_hex = null;
    protected Hex next_hex = null;
    [JsonRequired] protected int range { get; set; }
    public MovementBehaviour() : base() { path = new List<Hex>(); }
    public MovementBehaviour(Unit _unit)
    {
        unit = _unit;
        path = new List<Hex>();
    }
    public override void Execute()
    {
        if (path.Count == 0)
        {
            unit.events.OnEndMovement_Local?.Invoke(next_hex);
            NetworkManager.Instance.games[unit.match_id].game_events.OnEndMovement_Global?.Invoke(next_hex);
            Exit();
            return;
        }

        if(unit.IsDead())
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

            next_hex.TriggerModifier(unit);

            if (path.Count > 0)
                path.RemoveAt(0);

            if (path.Count > 0)
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
    public override void Enter()
    {
        base.Enter();
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
        time = Time.time;
        current_hex = _unit_hex;
        next_hex = null;
    }
}


