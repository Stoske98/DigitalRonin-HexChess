public class CritDamage : PhysicalDamage
{
    public int crit_multipl {get; set;}
    public CritDamage(Unit _unit, int _amount, float multiple) : base(_unit, _amount)
    {
        multiple /= 100;
        amount = (int)(amount * multiple);
    }
}


