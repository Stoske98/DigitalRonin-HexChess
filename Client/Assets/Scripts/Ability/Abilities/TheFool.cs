public class TheFool : PassiveAbility
{
    public TheFool() : base() { }
    public TheFool(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {

        unit.events.OnRecieveDamage_Local += OnRecieveDamage;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnRecieveDamage_Local -= OnRecieveDamage;
    }

    private void OnRecieveDamage(Hex _hex)
    {
        if (!unit.IsDeath())
        {
            unit.stats.current_health = 0;

            _hex.RemoveUnit();

            //--should be removed--
            unit.game_object.SetActive(false);
        }
        if(_hex != null)
            foreach (var hex_neighbor in _hex.neighbors)
                if (!hex_neighbor.IsWalkable() && hex_neighbor.GetUnit().class_type != unit.class_type)
                    hex_neighbor.GetUnit().RecieveDamage(new MagicDamage(unit, ability_data.amount));
    }
}

