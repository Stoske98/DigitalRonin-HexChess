public abstract class Damage
{
    public Unit unit { get; set; }
    public int amount { get; set; }

    public Damage(Unit _unit, int _amount)
    {
        unit = _unit;
        amount = _amount;
    }
}


