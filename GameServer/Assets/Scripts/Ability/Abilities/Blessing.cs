using Newtonsoft.Json;
using System.Collections.Generic;

public class Blessing : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Blessing() : base() { }
    public Blessing(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) {  }

    public override void Execute()
    {
        Heal(targetable_hex.GetUnit());
        Exit();

    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit aliance = hex.GetUnit();
            if (aliance != null && aliance.class_type == unit.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }

    private void Heal(Unit _unit)
    {
        if (_unit.stats.current_health + ability_data.amount > _unit.stats.max_health)
            _unit.stats.current_health = _unit.stats.max_health;
        else
            _unit.stats.current_health += ability_data.amount;
    }
}
