
public class MeleeAttack : AttackBehaviour
{
    public MeleeAttack() : base() { }
    public MeleeAttack(Unit _unit) : base(_unit) 
    {
    }
    public override void Execute()
    {
        if(unit.IsDead())
        {
            target = null;
            Exit();
            return;
        }

        if (UnityEngine.Time.time >= time + unit.stats.attack_speed)
        {
            Hex target_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(target);

            if (!target.IsDead())
                target.ReceiveDamage(damage);

            if (target_hex != null && target_hex.IsWalkable() && !unit.IsDead())
            {
                Hex unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(unit);
                unit.Move(unit_hex, target_hex);
            }
            target = null;
            Exit();
        }
    }
}

public class Attack : MeleeAttack
{
    public Attack() : base() { }
    public Attack(Unit _unit) : base(_unit)
    {
    }
    public override void Execute()
    {
        if (UnityEngine.Time.time >= time + unit.stats.attack_speed)
        {
            target.ReceiveDamage(damage);
            target = null;
            Exit();
        }
    }
}

