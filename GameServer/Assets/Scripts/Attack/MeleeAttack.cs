using System.Collections.Generic;
using static UnityEngine.UI.GridLayoutGroup;

public class MeleeAttack : AttackBehaviour
{
    public MeleeAttack() : base() { }
    public MeleeAttack(Unit _unit) : base(_unit) 
    {
    }
    public override void Execute()
    {
        if (UnityEngine.Time.time >= time + unit.stats.attack_speed)
        {
            Hex target_hex = NetworkManager.Instance.games[unit.match_id].GetHex(target);
            target.RecieveDamage(new PhysicalDamage(unit, unit.stats.damage));

            if (target_hex.IsWalkable())
            {
                Hex unit_hex = NetworkManager.Instance.games[unit.match_id].GetHex(unit);
                unit.Move(unit_hex, target_hex);
            }
            target = null;
            Exit();
        }
    }
}

