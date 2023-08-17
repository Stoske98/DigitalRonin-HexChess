public class PhysicalDamage : Damage
{
    public bool miss { get; set; }
    public PhysicalDamage(Unit _unit, int _amount) : base(_unit, _amount) 
    {
        miss = false;
    }
}


