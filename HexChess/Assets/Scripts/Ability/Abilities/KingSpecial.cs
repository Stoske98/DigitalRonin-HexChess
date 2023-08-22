using Newtonsoft.Json;
using System.Collections.Generic;

public class KingSpecial : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }

    [JsonRequired] private bool king_activate_special = false;
    public KingSpecial() : base() { }
    public KingSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }
    public override void Execute()
    {
        if (GameManager.Instance.game is ChallengeRoyaleGame game)
        {
            int counter = 0;
            foreach (var obj in GameManager.Instance.game.object_manager.objects)
            {
                if (obj is Unit death_unit && death_unit.stats.current_health == 0 && death_unit.class_type == unit.class_type) // check jester illu 
                    counter++;
            }

            //upgrade king
            UnityEngine.Debug.Log(unit.class_type.ToString() + " King: Challenge royale actiaveted");
            king_activate_special = true;
            game.Activate();

            Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
            unit.Move(_cast_unit_hex, targetable_hex);
        }
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();
        if (!king_activate_special)
        {
            Map map = GameManager.Instance.game.map;
            Hex grall_hex = map.GetHex(0, 0);

            if (grall_hex.IsWalkable() && map.HexesInRange(_unit_hex, ability_data.range).Contains(grall_hex))
                _available_moves.Add(grall_hex);
        }

        return _available_moves;
    }
}