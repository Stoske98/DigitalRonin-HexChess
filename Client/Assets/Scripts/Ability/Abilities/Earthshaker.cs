using System.Collections.Generic;

public class Earthshaker : InstantleAbility
{
    List<Unit> enemies;
    public Earthshaker() : base() { enemies = new List<Unit>(); }
    public Earthshaker(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { enemies = new List<Unit>(); }
    public override void Execute()
    {
        foreach (Unit enemy in enemies)
        {
            enemy.RecieveDamage(new MagicDamage(unit, ability_data.amount));
            if(!enemy.IsDeath())
                enemy.ccs.Add(new Stun(2));
        }
        enemies.Clear();
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                _ability_moves.Add(hex);

        return _ability_moves;
    }

    public override void SetAbility()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.GetHex(unit);
        if (_cast_unit_hex != null)
        {
            foreach (Hex _hex in GameManager.Instance.game.HexesInRange(_cast_unit_hex, ability_data.range))
                if (!_hex.IsWalkable() && _hex.GetUnit().class_type != unit.class_type)
                    enemies.Add(_hex.GetUnit());
        }
    }
}
