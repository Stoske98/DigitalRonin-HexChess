using System.Collections.Generic;

public class Stun : CC
{
    public Stun(int _max_cooldown) : base(_max_cooldown) { }
    public static bool IsStuned(List<CC> ccs)
    {
        foreach (CC cc in ccs)
            if (cc is Stun)
                return true;

        return false;
    }
}

