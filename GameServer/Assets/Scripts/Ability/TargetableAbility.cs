using System.Collections.Generic;

public abstract class TargetableAbility : Ability
{
    public TargetableAbility() : base() { }
    public TargetableAbility(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

}

