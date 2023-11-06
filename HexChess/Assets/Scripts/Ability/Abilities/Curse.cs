using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Curse : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    string path = "Prefabs/Wizard/Dark/Ability/Curse";
    GameObject vfx_prefab;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Curse() : base()
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Curse(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public override void Enter()
    {
        base.Enter();
        Object.Instantiate(vfx_prefab, targetable_hex.game_object.transform.position, Quaternion.identity);

    }
    public override void Execute()
    {
        if (Time.time >= time + 2)
        {
            Unit enemy = targetable_hex.GetUnit();
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

            if (!enemy.IsDead())
                targetable_hex.GetUnit().ccs.Add(new Disarm(unit, enemy, ability_data.cc));

            foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(targetable_hex, 1))
            {
                Unit _unit = hex.GetUnit();
                if (_unit != null && _unit.class_type != unit.class_type)
                {
                    _unit.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
                    if (!_unit.IsDead())
                        FearEnemy(hex);
                }
            }
            Exit();

        }
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if(enemy != null && unit.class_type != enemy.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
    private void FearEnemy(Hex _enemy_hex)
    {
        int column = _enemy_hex.coordinates.x - targetable_hex.coordinates.x;
        int row = _enemy_hex.coordinates.y - targetable_hex.coordinates.y;

        Hex hex = GameManager.Instance.game.map.GetHex(_enemy_hex.coordinates.x + column, _enemy_hex.coordinates.y + row);

        if (hex != null && hex.IsWalkable())
        {
            Unit enemy = _enemy_hex.GetUnit();
            enemy.Move(_enemy_hex, hex);
        }
    }

    public void Upgrade()
    {
        ability_data.amount += 1;
        ability_data.max_cooldown += 1;
    }
}
