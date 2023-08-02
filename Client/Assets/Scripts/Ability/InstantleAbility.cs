public abstract class InstantleAbility : Ability
{
    public InstantleAbility() : base() { }
    public InstantleAbility(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public abstract void SetAbility();
}

