public abstract class PassiveAbility : AbilityBehaviour, ISubscribe
{
    public PassiveAbility() : base() { }
    public PassiveAbility(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public abstract void RegisterEvents();
    public abstract void UnregisterEvents();
}

