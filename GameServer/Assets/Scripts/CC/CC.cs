using Newtonsoft.Json;
using UnityEngine;

public abstract class CC
{
    protected GameObject game_object { get; set; }
    [JsonRequired] protected Unit cast_unit { get; set; }
    [JsonRequired] protected Unit target_unit { get; set; }
    [JsonRequired] bool is_start_countdown { get; set; }
    public int max_cooldown { get; set; }
    public int current_cooldown { get; set; }

    public CC() { }

    public CC(Unit _cast_unit, Unit _target_unit, int _max_cooldown)
    {
        max_cooldown = current_cooldown = _max_cooldown;
        cast_unit = _cast_unit;
        target_unit = _target_unit;
        is_start_countdown = false;
    }

    public void UpdateCooldown()
    {
        if (!is_start_countdown)
        {
            is_start_countdown = true;
            return;
        }
        if (current_cooldown > 0)
            current_cooldown--;
    }

    public bool HasCooldownExpired()
    {
        if (current_cooldown == 0)
        {
            if (game_object != null)
                Object.Destroy(game_object);

            return true;
        }
        else
            return false;
    }
    public abstract void Init();
}

