public abstract class TargetableAbility : Ability
{
    protected Hex targetable_hex;
    public TargetableAbility() : base() { }
    public TargetableAbility(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public abstract void SetAbility(Hex _targetable_hex);
}

