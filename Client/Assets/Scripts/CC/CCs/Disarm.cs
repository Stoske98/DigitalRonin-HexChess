using System.Collections.Generic;

public class Disarm : CC
{
    public Disarm() : base() { }
    public Disarm(int _max_cooldown) : base(_max_cooldown) { }
    public static bool IsDissarmed(List<CC> ccs)
    {
        foreach (CC cc in ccs)
            if (cc is Disarm)
                return true;

        return false;
    }
}

