using Newtonsoft.Json;
using System;
using System.Diagnostics;

public class TheTricsOfTrade : PassiveAbility, IUpgradable
{
    public TheTricsOfTrade() : base() { }
    public TheTricsOfTrade(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
        unit.AddAttackBehaviour(new MeleeAttack(unit));
        if(unit.level >= 2)
            unit.events.OnStartAttack_local += OnStartAttack;
    }

    public override void UnregisterEvents()
    {
        unit.AddAttackBehaviour(new NoAttack(unit));
        if (unit.level >= 2)
            unit.events.OnStartAttack_local -= OnStartAttack;
    }

    public void Upgrade()
    {
        ability_data.amount = 25;
        unit.events.OnStartAttack_local += OnStartAttack;
    }

    private Damage OnStartAttack(Damage damage)
    {
        if (NetworkManager.Instance.games[unit.match_id].random_seeds_generator.PercentCalc(ability_data.amount))
            return new CritDamage(damage.unit, damage.amount, 150);

        return damage;
    }
}

public class TheTricksOfTradeFake : PassiveAbility, IUpgradable
{
    public TheTricksOfTradeFake() : base() { }
    public TheTricksOfTradeFake(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
    }

    public override void UnregisterEvents()
    {
    }

    public void Upgrade()
    {
        ability_data.amount = 25;
    }
}

