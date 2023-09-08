using UnityEngine;

public class Disarm : CC
{
    public Disarm() : base() { }
    public Disarm(Unit _cast_unit, Unit _target_unit, int _max_cooldown) : base(_cast_unit, _target_unit, _max_cooldown)
    {
        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CC/Disarm/Disarm"));
        game_object.transform.position = target_unit.game_object.transform.position;
        game_object.transform.SetParent(target_unit.game_object.transform);
    }
    public static bool IsDissarmed(Unit _unit)
    {
        foreach (CC cc in _unit.ccs)
            if (cc is Disarm)
                return true;

        return false;
    }

    public override void Init()
    {
        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CC/Disarm/Disarm"));
        game_object.transform.position = target_unit.game_object.transform.position;
        game_object.transform.SetParent(target_unit.game_object.transform);
    }
}

