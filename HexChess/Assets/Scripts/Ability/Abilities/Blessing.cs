using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Blessing : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    string path = "Prefabs/Wizard/Light/Ability/Blessing";
    GameObject vfx_prefab;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Blessing() : base()
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Blessing(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public override void Execute()
    {
        Heal(targetable_hex.GetUnit());
        Exit();

    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit aliance = hex.GetUnit();
            if (aliance != null && aliance.class_type == unit.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }

    private void Heal(Unit _unit)
    {
        if (_unit.stats.current_health + ability_data.amount > _unit.stats.max_health)
            _unit.stats.current_health = _unit.stats.max_health;
        else
            _unit.stats.current_health += ability_data.amount;

        GameManager.Instance.game.game_events.OnChangeUnitData_Global?.Invoke(_unit);
        Object.Instantiate(vfx_prefab, _unit.game_object.transform.position, Quaternion.identity);
    }

    public void Upgrade()
    {
        ability_data.max_cooldown += 1;
        ability_data.amount += 1;
    }
}

