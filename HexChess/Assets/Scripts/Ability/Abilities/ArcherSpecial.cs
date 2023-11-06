public class ArcherSpecial : PassiveAbility
{
    public ArcherSpecial() : base() { }
    public ArcherSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) {}

    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        if (unit.GetBehaviour<ArcherRangedAttack>() == null)
        {
            unit.stats.attack_range = 2;
            if(unit.class_type == ClassType.Light)
                unit.AddAttackBehaviour(new ArcherRangedAttack(unit, "Prefabs/Archer/Light/Projectil/prefab"));
            else
                unit.AddAttackBehaviour(new ArcherRangedAttack(unit, "Prefabs/Archer/Dark/Projectil/prefab"));
        }
    }

    public override void UnregisterEvents()
    {
        unit.stats.attack_range = 1;
        unit.AddAttackBehaviour(new MeleeAttack(unit));
    }
}

