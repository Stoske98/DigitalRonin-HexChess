﻿public abstract class CC
{
    public int max_cooldown { get; set; }

    public int current_cooldown { get; set; }

    public CC() { }

    public CC(int _max_cooldown)
    {
        max_cooldown = current_cooldown = _max_cooldown;
    }

    public void UpdateCooldown()
    {
        if (current_cooldown > 0)
            current_cooldown--;
    }

    public bool HasCooldownExpired()
    {
        return current_cooldown == 0 ? true : false;
    }
}
