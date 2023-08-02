public abstract class PassiveAbility : AbilityBehaviour, ISubscribe
{
    public PassiveAbility() : base() { }
    public PassiveAbility(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public abstract void RegisterEvents();
    public abstract void UnregisterEvents();
}

