using System.Collections.Generic;

public class KnightPassive : PassiveAbility
{
    public KnightPassive() : base() { }
    public KnightPassive(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovement;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovement;
    }

    private void OnStartMovement(Hex _from_hex, Hex _target_hex)
    {
        Behaviour behaviour = unit.GetBehaviour<MovementBehaviour>();
        if (behaviour != null && behaviour is KnightMovement)
        {
            List<Unit> enemy_units = new List<Unit>();

            if(_from_hex != null)
            {
                foreach (Hex hex_neighbor in _target_hex.neighbors)
                {
                    if (_from_hex.neighbors.Contains(hex_neighbor) && !hex_neighbor.IsWalkable())
                    {
                        if (!hex_neighbor.IsWalkable() && hex_neighbor.GetUnit().class_type != unit.class_type)
                            enemy_units.Add(hex_neighbor.GetUnit());
                    }
                }
            }

            foreach (Unit enemy in enemy_units)
                enemy.RecieveDamage(new MagicDamage(unit, ability_data.amount));
        }
    }
}
