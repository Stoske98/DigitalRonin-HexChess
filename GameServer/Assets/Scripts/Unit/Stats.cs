public class Stats
{
    public int max_health { get; set; }
    public int current_health { get; set; }
    public int damage { get; set; } 
    public int attack_range { get; set; }
    public float attack_speed { get; set; }
}
public class StatsUpdate
{
    public int increase_max_health { get; set; }
    public int increase_damage { get; set; }
    public int increase_attack_range { get; set; }
    public float increase_attack_speed { get; set; }
}
