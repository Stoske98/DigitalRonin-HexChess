using System.Collections.Generic;
using UnityEngine;

public class Joust : TargetableAbility
{
    public Joust() : base() { }
    public Joust(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        Unit enemy = targetable_hex.GetUnit();
        if(enemy != null)
        {
            enemy.RecieveDamage(new MagicDamage(unit, ability_data.amount));

            Hex _cast_unit_hex = NetworkManager.Instance.games[unit.match_id].GetHex(unit);
            if(_cast_unit_hex != null)
            {
                unit.Move(_cast_unit_hex, targetable_hex);

                if (!enemy.IsDeath())
                    enemy.Move(targetable_hex, _cast_unit_hex);
            }
        }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type &&
                (Mathf.Abs(hex.coordinates.x - _unit_hex.coordinates.x) == ability_data.range
                || Mathf.Abs(hex.coordinates.y - _unit_hex.coordinates.y) == ability_data.range
                || Mathf.Abs(hex.S - _unit_hex.S) == ability_data.range))
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }
}
