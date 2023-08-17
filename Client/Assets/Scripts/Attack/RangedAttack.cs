using System.Reflection;
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
            Missile missile = new Missile(target, damage, 55, 7);
            GameManager.Instance.game.object_manager.AddObject(missile);
            target = null;

            Exit();
        }
    }
}

