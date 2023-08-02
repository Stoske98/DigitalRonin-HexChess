using System.Collections.Generic;

public class Blessing : TargetableAbility//, IUpgradable
{
    /*public Curse curse;
    Queue<LevelUpdateData> list_updates;*/
    public Blessing() : base() { }
    public Blessing(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { /*curse = new Curse(unit, new AbilityData());*/ }
    public override void Execute()
    {
        Heal(targetable_hex.GetUnit());
        Exit();

    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.HexesInRange(_unit_hex, ability_data.range))
            if (!hex.IsWalkable() && hex.GetUnit().class_type == unit.class_type)
                _available_moves.Add(hex);

        return _available_moves;
    }

    public override void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }

    private void Heal(Unit _unit)
    {
        if (_unit.stats.current_health + ability_data.amount > _unit.stats.max_health)
            _unit.stats.current_health = _unit.stats.max_health;
        else
            _unit.stats.current_health += ability_data.amount;
    }

  /*  public void Upgrade()
    {
        if(list_updates.Count > 0)
        {
            LevelUpdateData update_ability_data = list_updates.Dequeue();
            ability_data.amount += update_ability_data.amount;
            ability_data.max_cooldown += update_ability_data.cooldowm;
            ability_data.cc += update_ability_data.cc;
            ability_data.range += update_ability_data.range;
        }
        else 
        {
            for (int i = 0; i < unit.behaviours.Count; i++)
            {
                if (unit.behaviours[i] == this)
                    unit.behaviours[i] = curse;
            }
        }
    }*/
}
