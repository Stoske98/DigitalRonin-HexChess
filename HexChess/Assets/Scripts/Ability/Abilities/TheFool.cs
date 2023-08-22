using System.Collections.Generic;

public class TheFool : PassiveAbility
{
    public TheFool() : base() { }
    public TheFool(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

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
        foreach (var hex_neighbor in _hex.GetNeighbors(GameManager.Instance.game.map))
        {
            Unit enemy = hex_neighbor.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
        }
    }
}

public class TheFakeFool : PassiveAbility
{
    public TheFakeFool() : base() { }
    public TheFakeFool(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
    }

    public override void UnregisterEvents()
    {
    }
}


public class TheFoolFinal : InstantleAbility, ISubscribe
{
    List<Unit> enemies;
    public TheFoolFinal() : base() { enemies = new List<Unit>(); }
    public TheFoolFinal(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { enemies = new List<Unit>(); }
    public void RegisterEvents()
    {

        unit.events.OnRecieveDamage_Local += OnRecieveDamage;
    }

    public void UnregisterEvents()
    {
        unit.events.OnRecieveDamage_Local -= OnRecieveDamage;
    }
    public override void Execute()
    {
        Explode();
        Remove();
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                _ability_moves.Add(hex);
        }

        return _ability_moves;
    }

    public override void SetAbility()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
        if (_cast_unit_hex != null)
        {
            foreach (Hex _hex in GameManager.Instance.game.map.HexesInRange(_cast_unit_hex, ability_data.range))
            {
                Unit enemy = _hex.GetUnit();
                if (enemy != null && enemy.class_type != unit.class_type)
                    enemies.Add(enemy);

            }
        }
    }
    private void OnRecieveDamage(Hex _hex)
    {
        foreach (Hex neighbor_hex in GameManager.Instance.game.map.HexesInRange(_hex, ability_data.range))
        {
            Unit enemy = neighbor_hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                enemies.Add(enemy);
        }

        Explode();
    }
    private void Explode()
    {
        foreach (Unit enemy in enemies)
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

        enemies.Clear();
    }

    private void Remove()
    {
        GameManager.Instance.game.map.GetHex(unit)?.RemoveObject(unit);
        unit.stats.current_health = 0;
        IObject.ObjectVisibility(unit, Visibility.NONE);
        unit.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
    }
}
public class TheFakeFoolFinal : InstantleAbility
{
    public TheFakeFoolFinal() : base() { }
    public TheFakeFoolFinal(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {

    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }

    public override void SetAbility()
    {
    }
}

