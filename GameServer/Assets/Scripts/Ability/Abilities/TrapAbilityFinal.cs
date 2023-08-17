using System.Collections.Generic;

public class TrapAbilityFinal : TargetableAbility, ITargetMultipleHexes
{
    public int max_hexes { get; set; }
    public bool has_condition { get; set; }
    public List<Hex> targetable_hexes { get; set; }
    public TrapAbilityFinal() : base()
    {
        targetable_hexes = new List<Hex>();
    }
    public TrapAbilityFinal(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
        targetable_hexes = new List<Hex>();
        max_hexes = 2;
        has_condition = false;
    }
    public override void Execute()
    {
        Game game = NetworkManager.Instance.games[unit.match_id];
        foreach (var targetable_hex in targetable_hexes)
        {
            Trap trap = new Trap(unit, this);
            game.map.PlaceObject(trap, targetable_hex.coordinates.x, targetable_hex.coordinates.y);
            game.object_manager.AddObject(trap);

        }
        targetable_hexes.Clear();
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, ability_data.range))
        {
            if (hex != null && hex.IsWalkable())
            {
                bool trapFound = false;

                foreach (var obj in hex.objects)
                {
                    if (obj is Trap trap)
                    {
                        trapFound = true;
                        break;
                    }
                }

                if (!trapFound)
                    _available_moves.Add(hex);
            }
        }

        return _available_moves;
    }
}