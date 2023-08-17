using System.Collections.Generic;

public class Disarm : CC
{
    public Disarm(int _max_cooldown) : base(_max_cooldown) { }
    public static bool IsDissarmed(Unit _unit)
    {
        foreach (CC cc in _unit.ccs)
            if (cc is Disarm)
                return true;

        return false;
    }
}

