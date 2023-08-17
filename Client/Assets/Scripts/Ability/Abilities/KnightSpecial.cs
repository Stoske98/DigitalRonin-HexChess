using System.Collections.Generic;

public class KnightSpecial : PassiveAbility, IUpgradable
{
    public KnightSpecial() : base() { }
    public KnightSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

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
                foreach (Hex hex_neighbor in _target_hex.GetNeighbors(GameManager.Instance.game.map))
                {
                    if (_from_hex.GetNeighbors(GameManager.Instance.game.map).Contains(hex_neighbor) && !hex_neighbor.IsWalkable())
                    {
                        Unit enemy = hex_neighbor.GetUnit();
                        if (enemy != null && enemy.class_type != unit.class_type)
                            enemy_units.Add(enemy);
                    }
                }
            }
            if (unit.level < 3)
            {
                if (enemy_units.Count == 1)
                {
                    enemy_units[0].ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                }
                else if (enemy_units.Count == 2)
                {
                    if (GameManager.Instance.game.random_seeds_generator.PercentCalc(50))
                        enemy_units[0].ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                    else
                        enemy_units[1].ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                }

            }
            else
                foreach (var enemy in enemy_units)
                    enemy.ReceiveDamage(new PhysicalDamage(unit, unit.stats.damage));
        }
    }

    public void Upgrade()
    {
    }
}
