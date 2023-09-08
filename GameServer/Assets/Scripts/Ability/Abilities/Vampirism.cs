using Newtonsoft.Json;
using System.Collections.Generic;

public class Vampirism : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Vampirism() : base() { }
    public Vampirism(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        List<Hex> longest_enemy_path = PathFinding.PathFinder.BFS_LongestEnemyUnitPath(targetable_hex, unit, map, 5);
        for (int i = 0; i < 5; i++)
        {
            if (i < longest_enemy_path.Count)
            {
                Unit enemy = longest_enemy_path[i].GetUnit();
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

                if (unit.stats.current_health + ability_data.amount > unit.stats.max_health)
                    unit.stats.current_health = unit.stats.max_health;
                else
                    unit.stats.current_health += ability_data.amount;
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
            if (enemy != null && unit.class_type != enemy.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
}
