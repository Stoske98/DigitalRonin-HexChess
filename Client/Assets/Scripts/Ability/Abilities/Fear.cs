using System.Collections.Generic;

public class Fear : InstantleAbility
{
    List<Unit> enemies;
    public Fear() : base() { enemies = new List<Unit>(); }
    public Fear(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { enemies = new List<Unit>(); }

    public override void Execute()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
        foreach (Unit enemy in enemies)
        {
            enemy.ReceiveDamage(new MagicDamage(unit,ability_data.amount));
            if (!enemy.IsDead())
            {
                Hex _enemy_hex = GameManager.Instance.game.map.GetHex(enemy);
                if(_enemy_hex != null)
                    FearEnemy(_cast_unit_hex, _enemy_hex);

            }
        }

        enemies.Clear();
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

    private void FearEnemy(Hex _cast_unit_hex, Hex _enemy_hex)
    {
        int column = _enemy_hex.coordinates.x - _cast_unit_hex.coordinates.x;
        int row = _enemy_hex.coordinates.y - _cast_unit_hex.coordinates.y;

        Hex hex = GameManager.Instance.game.map.GetHex(_enemy_hex.coordinates.x + column, _enemy_hex.coordinates.y + row);

        if (hex != null && hex.IsWalkable())
        {
            Unit enemy = _enemy_hex.GetUnit();
            enemy.Move(_enemy_hex, hex);
        }
    }
}
