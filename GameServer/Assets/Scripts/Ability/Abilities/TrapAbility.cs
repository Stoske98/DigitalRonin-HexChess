using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TrapAbility : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore]public Hex targetable_hex { get; set; }
    public TrapAbility() : base() { }
    public TrapAbility(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        Trap trap = Spawner.CreateTrap(unit, this);
        
        Game game = NetworkManager.Instance.games[unit.match_id];
        game.map.PlaceObject(trap, targetable_hex.coordinates.x, targetable_hex.coordinates.y);
        game.object_manager.AddObject(trap);

        IObject.ObjectVisibility(trap, trap.visibility);
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
