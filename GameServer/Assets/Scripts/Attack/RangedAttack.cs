using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AttackBehaviour
{
    public RangedAttack() : base() { }
    public RangedAttack(Unit _unit) : base(_unit) 
    {
    }

    public override void Execute()
    {
        if (Time.time >= time + unit.stats.attack_speed)
        {
            Missile missile = new Missile(target, damage, 55, 10);
            NetworkManager.Instance.games[unit.match_id].object_manager.AddObject(missile);
            target = null;

            Exit();
        }
    }

}

public class ArcherRangedAttack : RangedAttack
{
    public ArcherRangedAttack() : base() { }
    public ArcherRangedAttack(Unit _unit) : base(_unit)
    {
    }

    public override List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        List<Hex> hexes = new List<Hex>();
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        for (int i = 1; i < unit.stats.attack_range; i++)
        {
            foreach (var diagonal in map.diagonals_neighbors_vectors)
            {
                Hex hex = map.GetHex(_unit_hex.coordinates.x + diagonal.x * i, _unit_hex.coordinates.y + diagonal.y * i);
                if(hex != null)
                {
                    Unit enemy = hex.GetUnit();
                    if (enemy != null && enemy.class_type != unit.class_type)
                        hexes.Add(hex);
                }
            }
        }
        return hexes;
    }

}
