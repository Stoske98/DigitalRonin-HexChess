using System;

public class TheTricsOfTrade : PassiveAbility
{
    public TheTricsOfTrade() : base() { }
    public TheTricsOfTrade(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
       // unit.attack_behaviour = new MeleeAttack(unit);
    }

    public override void UnregisterEvents()
    {
       // unit.attack_behaviour = new NoAttack(unit); 
    }
}

