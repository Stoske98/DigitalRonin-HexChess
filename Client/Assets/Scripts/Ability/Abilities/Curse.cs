using Newtonsoft.Json;
using System.Collections.Generic;

public class Curse : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Curse() : base() {  }
    public Curse(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        targetable_hex.GetUnit().ReceiveDamage(new MagicDamage(unit, ability_data.amount));
        if (!targetable_hex.IsWalkable())
            targetable_hex.GetUnit().ccs.Add(new Disarm(ability_data.cc));

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(targetable_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
            {
                Unit enemy = hex.GetUnit();
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                if (!enemy.IsDead())
                    FearEnemy(hex);
            }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                _available_moves.Add(hex);

        return _available_moves;
    }
    private void FearEnemy(Hex _enemy_hex)
    {
        int column = _enemy_hex.coordinates.x - targetable_hex.coordinates.x;
        int row = _enemy_hex.coordinates.y - targetable_hex.coordinates.y;

        Hex hex = GameManager.Instance.game.map.GetHex(_enemy_hex.coordinates.x + column, _enemy_hex.coordinates.y + row);

        if (hex != null && hex.IsWalkable())
        {
            Unit enemy = _enemy_hex.GetUnit();
            enemy.Move(_enemy_hex, hex);
        }
    }
}
