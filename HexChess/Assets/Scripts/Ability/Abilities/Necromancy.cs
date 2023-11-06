using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Necromancy : TargetableAbility, ITargetableSingleHex
{
    string path_sacrafice = "Prefabs/Wizard/Dark/Ability/Sacrafice";
    GameObject vfx_prefab_sacrafice;
    string path_heal = "Prefabs/Wizard/Dark/Ability/Heal";
    GameObject vfx_prefab_heal;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Necromancy() : base()
    {
        vfx_prefab_sacrafice = Resources.Load<GameObject>(path_sacrafice);
        vfx_prefab_heal = Resources.Load<GameObject>(path_heal);
    }
    public Necromancy(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        vfx_prefab_sacrafice = Resources.Load<GameObject>(path_sacrafice);
        vfx_prefab_heal = Resources.Load<GameObject>(path_heal);
    }
    public override void Execute()
    {
        if(targetable_hex.GetUnit().class_type == unit.class_type)
        {
            Heal(targetable_hex.GetUnit());
            unit.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
            DeathSphereProject sphere = Object.Instantiate(vfx_prefab_sacrafice, unit.game_object.transform.position, Quaternion.identity).GetComponent<DeathSphereProject>();
            sphere.Prepare(unit.game_object.transform.position, targetable_hex.game_object.transform.position, 4, 60);
        }
        else
        {
            Heal(unit);
            targetable_hex.GetUnit().ReceiveDamage(new MagicDamage(unit, ability_data.amount));
            DeathSphereProject sphere = Object.Instantiate(vfx_prefab_heal, targetable_hex.game_object.transform.position, Quaternion.identity).GetComponent<DeathSphereProject>();
            sphere.Prepare(targetable_hex.game_object.transform.position, unit.game_object.transform.position, 4, 60);
        }
        Exit();
    }
    
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
         {
            Unit _unit = hex.GetUnit();
            if (_unit != null)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
    private void Heal(Unit unit)
    {
        if (unit.stats.current_health + ability_data.amount > unit.stats.max_health)
            unit.stats.current_health = unit.stats.max_health;
        else
            unit.stats.current_health += ability_data.amount;
    }
    
}