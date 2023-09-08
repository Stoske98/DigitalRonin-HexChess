using UnityEngine;

public class Stun : CC
{
    public Stun() : base() { }
    public Stun(Unit _cast_unit, Unit _target_unit, int _max_cooldown) : base(_cast_unit, _target_unit, _max_cooldown) 
    {
        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CC/Stun/Stun"));
        game_object.transform.position = target_unit.game_object.transform.position;
        game_object.transform.SetParent(target_unit.game_object.transform);
    }
    public static bool IsStuned(Unit _unit)
    {
        foreach (CC cc in _unit.ccs)
            if (cc is Stun)
                return true;

        return false;
    }

    public override void Init()
    {
        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/CC/Stun/Stun"));
        game_object.transform.position = target_unit.game_object.transform.position;
        game_object.transform.SetParent(target_unit.game_object.transform);
    }
}

