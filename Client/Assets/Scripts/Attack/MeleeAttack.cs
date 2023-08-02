using System.Collections.Generic;

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
            Hex target_hex = GameManager.Instance.game.GetHex(target);
            target.RecieveDamage(new PhysicalDamage(unit, unit.stats.damage));

            if (target_hex.IsWalkable())
            {
                Hex unit_hex = GameManager.Instance.game.GetHex(unit);
                unit.Move(unit_hex, target_hex);
            }
            target = null;
            Exit();
        }
    }
}

