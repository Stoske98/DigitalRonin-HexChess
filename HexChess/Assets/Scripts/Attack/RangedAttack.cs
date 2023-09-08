using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RangedAttack : AttackBehaviour
{
    protected GameObject prefab { get; set; } 
    public RangedAttack() : base() { }
    public RangedAttack(Unit _unit, string path) : base(_unit) 
    {
        prefab = Resources.Load<GameObject>(path);
    }
   
    public override void Execute()
    {
        if (Time.time >= time + unit.stats.attack_speed)
        {
            if (prefab == null)
                LoadPrefab();
            Missile missile = new Missile(target, damage, prefab, 55, 10);
            GameManager.Instance.game.object_manager.AddObject(missile);
            target = null;

            Exit();
        }
    }

    public virtual void LoadPrefab()
    {

    }
}

public class ArcherRangedAttack : RangedAttack
{
    public ArcherRangedAttack() : base() { }
    public ArcherRangedAttack(Unit _unit, string path) : base(_unit, path)
    {
    }

    public override List<Hex> GetAttackMoves(Hex _unit_hex)
    {
        List<Hex> hexes = new List<Hex>();
        Map map = GameManager.Instance.game.map;
        for (int i = 1; i < unit.stats.attack_range; i++)
        {
            foreach (var diagonal in map.diagonals_neighbors_vectors)
            {
                Hex hex = map.GetHex(_unit_hex.coordinates.x + diagonal.x * i, _unit_hex.coordinates.y + diagonal.y * i);
                if (hex != null)
                {
                    Unit enemy = hex.GetUnit();
                    if (enemy != null && enemy.class_type != unit.class_type)
                        hexes.Add(hex);
                }
            }
        }
        return hexes;
    }

    public override void LoadPrefab() 
    {
        if(unit.class_type == ClassType.Dark)
            prefab = Resources.Load<GameObject>("Prefabs/Archer/Dark/Projectil/prefab");
        else
            prefab = Resources.Load<GameObject>("Prefabs/Archer/Light/Projectil/prefab");
    }

}