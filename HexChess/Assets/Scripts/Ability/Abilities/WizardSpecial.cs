using System;

public class WizardSpecial : PassiveAbility
{
    public WizardSpecial() : base() { }
    public WizardSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override void RegisterEvents()
    {
        if (unit.GetBehaviour<TeleportMovement>() == null)
            unit.AddMovementBehaviour(new TeleportMovement(unit, ability_data.range));
    }

    public override void UnregisterEvents()
    {
        unit.AddMovementBehaviour(new NoMovement(unit));
    }
}
