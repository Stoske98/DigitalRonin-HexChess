public class AbilityData
{
    /* public string name;
     public string description;*/
    public int level = 0;
    public int max_cooldown = 0;
    public int current_cooldown = 0;

    public int range = 0;
    public int amount = 0;
    public int cc;
}

public class AbilityUpdate
{
    public int decrease_cooldowm { get; set; }
    public int increase_range { get; set; }
    public int increase_amount { get; set; }
    public int increase_cc { get; set; }
}
