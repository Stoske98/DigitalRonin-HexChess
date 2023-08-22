public abstract class InstantleAbility : Ability
{
    public InstantleAbility() : base() { }
    public InstantleAbility(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public abstract void SetAbility();
}

