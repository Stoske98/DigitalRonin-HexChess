using Newtonsoft.Json;

public class Hunt : PassiveAbility, IUpgradable
{
    [JsonRequired] private bool activated { get; set; }
    private int max_damage_increment { get; set; }
    [JsonRequired] private int current_damage_increment { get; set; }
    public Hunt() : base() { }
    public Hunt(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { activated = false; max_damage_increment = 3; current_damage_increment = 0; }
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovement;
        unit.events.OnBeforeReceivingDamage_Local += OnBeforeReceivingDamage;
        if (unit.level >= 2)
            unit.events.OnStartAttack_local += OnStartAttack;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovement;
        unit.events.OnBeforeReceivingDamage_Local -= OnBeforeReceivingDamage;
        if (unit.level >= 2)
            unit.events.OnStartAttack_local -= OnStartAttack;
    }

    private void OnBeforeReceivingDamage(Damage damage)
    {
        if (damage is PhysicalDamage physical_damage)
            if (GameManager.Instance.game.random_seeds_generator.PercentCalc(ability_data.amount))
                physical_damage.miss = true;
            else
                activated = false;
    }
    private void OnStartMovement(Hex hex1, Hex hex2)
    {
        if (unit.level >= 2 && max_damage_increment > current_damage_increment)
            current_damage_increment += 1;

        activated = true;
    }
    private Damage OnStartAttack(Damage damage)
    {
        damage.amount += current_damage_increment;
        current_damage_increment = 0;
        return damage;
    }

    public void Upgrade()
    {
        unit.events.OnStartAttack_local += OnStartAttack;
    }
}