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
            target.RecieveDamage(new PhysicalDamage(unit, unit.stats.damage));
            target = null;

            Exit();
        }
    }

}

