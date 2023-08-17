using System;

public class WizardSpecial : PassiveAbility
{
    public WizardSpecial() : base() { }
    public WizardSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override void RegisterEvents()
    {
       // unit.behaviours.Add(new TeleportMovement(unit, ability_data.range));
    }

    public override void UnregisterEvents()
    {
       // unit.movement_behaviour = movement;
    }
}
