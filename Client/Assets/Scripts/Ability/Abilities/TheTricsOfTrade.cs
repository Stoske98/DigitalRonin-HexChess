using Newtonsoft.Json;
using System;

public class TheTricsOfTrade : PassiveAbility, IUpgradable
{
    [JsonRequired] private bool upgraded { get; set; }
    public TheTricsOfTrade() : base() { }
    public TheTricsOfTrade(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { upgraded = false; }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {

        unit.AddAttackBehaviour(new MeleeAttack(unit));
        if (upgraded)
            unit.events.OnStartAttack_local += OnStartAttack;
    }

    public override void UnregisterEvents()
    {
        unit.AddAttackBehaviour(new NoAttack(unit));
        if (upgraded)
            unit.events.OnStartAttack_local -= OnStartAttack;
    }
    public void Upgrade()
    {
        upgraded = true;
        ability_data.amount = 25;
        unit.events.OnStartAttack_local += OnStartAttack;
    }
    private Damage OnStartAttack(Damage damage)
    {
        if (GameManager.Instance.game.random_seeds_generator.PercentCalc(ability_data.amount))
            return new CritDamage(damage.unit, damage.amount, 150);

        return damage;
    }
}
public class TheTricksOfTradeFake : PassiveAbility, IUpgradable
{
    public TheTricksOfTradeFake() : base() { }
    public TheTricksOfTradeFake(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

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
