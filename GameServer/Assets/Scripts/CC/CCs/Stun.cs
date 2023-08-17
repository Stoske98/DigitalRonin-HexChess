using System.Collections.Generic;

public class Stun : CC
{
    public Stun(int _max_cooldown) : base(_max_cooldown) { }
    public static bool IsStuned(Unit _unit)
    {
        foreach (CC cc in _unit.ccs)
            if (cc is Stun)
                return true;

        return false;
    }
}

