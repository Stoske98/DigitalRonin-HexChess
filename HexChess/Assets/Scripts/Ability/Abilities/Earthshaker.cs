using System.Collections.Generic;
using UnityEngine;

public class Earthshaker : InstantleAbility
{
    string path = "Prefabs/Tank/Light/Ability/Earthshaker";
    GameObject vfx_prefab;
    List<Unit> enemies;
    public Earthshaker() : base() 
    { 
        enemies = new List<Unit>();
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Earthshaker(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) 
    { 
        enemies = new List<Unit>();
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public override void Execute()
    {
        foreach (Unit enemy in enemies)
        {
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
            if(!enemy.IsDead())
                enemy.ccs.Add(new Stun(unit, enemy, ability_data.cc));
        }
        Object.Instantiate(vfx_prefab,unit.game_object.transform.position,Quaternion.identity);
        enemies.Clear();
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                _ability_moves.Add(hex);
        }

        return _ability_moves;
    }

    public override void SetAbility()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
        if (_cast_unit_hex != null)
        {
            foreach (Hex _hex in GameManager.Instance.game.map.HexesInRange(_cast_unit_hex, ability_data.range))
            {
                Unit enemy = _hex.GetUnit();
                if (enemy != null && enemy.class_type != unit.class_type)
                    enemies.Add(enemy);

            }
        }
    }
}
