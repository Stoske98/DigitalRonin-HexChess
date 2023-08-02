public interface ICooldown
{
    int max_cooldown { get; }
    int current_cooldown { get; set; }
    abstract void UpdateCooldown();
    bool HasCooldownExpired();
}

