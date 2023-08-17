using Newtonsoft.Json;
using System.Collections.Generic;

public class Curse : TargetableAbility, ITargetableSingleHex
{
    List<Unit> enemies;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Curse() : base() {  }
    public Curse(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        Unit enemy = targetable_hex.GetUnit();
        enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

        if (!enemy.IsDead())
            targetable_hex.GetUnit().ccs.Add(new Disarm(ability_data.cc));

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(targetable_hex, ability_data.range))
        {
            Unit _unit = hex.GetUnit();
            if (_unit != null && _unit.class_type != unit.class_type)
            {
                _unit.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                if (!_unit.IsDead())
                    FearEnemy(hex);
            }
        }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
    private void FearEnemy(Hex _enemy_hex)
    {
        int column = _enemy_hex.coordinates.x - targetable_hex.coordinates.x;
        int row = _enemy_hex.coordinates.y - targetable_hex.coordinates.y;

        Hex hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(_enemy_hex.coordinates.x + column, _enemy_hex.coordinates.y + row);

        if (hex != null && hex.IsWalkable())
        {
            Unit enemy = _enemy_hex.GetUnit();
            enemy.Move(_enemy_hex, hex);
        }
    }
}
