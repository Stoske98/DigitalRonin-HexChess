using System.Collections.Generic;

public abstract class Ability : AbilityBehaviour
{
    public Ability() : base() { }
    public Ability(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public abstract List<Hex> GetAbilityMoves(Hex _unit_hex);
    public void UpdateCooldown()
    {
        if (ability_data.current_cooldown > 0)
            ability_data.current_cooldown--;
    }
    public bool HasCooldownExpired()
    {
        return ability_data.current_cooldown == 0;
    }
}

