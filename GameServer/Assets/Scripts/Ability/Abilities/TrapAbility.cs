using System.Collections.Generic;

public class TrapAbility : TargetableAbility
{
    public TrapAbility() : base() { }
    public TrapAbility(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }
    public override void Execute()
    {
        Trap trap = new Trap(unit, this);
        targetable_hex.PlaceObject(trap);
        NetworkManager.Instance.games[unit.match_id].objects.Add(trap);

        Exit();

    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].HexesInRange(_unit_hex, ability_data.range))
            if (hex != null && hex.IsWalkable())
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }
}