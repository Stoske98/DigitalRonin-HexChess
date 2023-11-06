using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KingSpecial : TargetableAbility, ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }

    [JsonRequired] private bool king_activate_special = false;
    string path_light = "Prefabs/King/Light/Ability/SpecialAbility/prefab";
    string path_dark = "Prefabs/King/Dark/Ability/SpecialAbility/prefab";
    GameObject vfx_prefab;
    public KingSpecial() : base()
    {
    }
    public KingSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        if(unit.class_type == ClassType.Light)
            vfx_prefab = Resources.Load<GameObject>(path_light);
        else
            vfx_prefab = Resources.Load<GameObject>(path_dark);
    }
    public override void Execute()
    {
        if (GameManager.Instance.game is ChallengeRoyaleGame game)
        { 
            //upgrade king
            UnityEngine.Debug.Log(unit.class_type.ToString() + " King: Challenge royale actiaveted");
            king_activate_special = true;
            game.Activate();

            Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);
            Unit aliance = targetable_hex.GetUnit();

            if(vfx_prefab == null)
            {
                if (unit.class_type == ClassType.Light)
                    vfx_prefab = Resources.Load<GameObject>(path_light);
                else
                    vfx_prefab = Resources.Load<GameObject>(path_dark);
            }

            UnityEngine.Object.Instantiate(vfx_prefab, targetable_hex.game_object.transform.position, Quaternion.identity);
            if (aliance != null)
            {
                aliance.ReceiveDamage(new MagicDamage(unit, aliance.stats.max_health));
                targetable_hex.RemoveObject(aliance);
                //IObject.ObjectVisibility(aliance, Visibility.NONE);

                BlinkMovement blink = new BlinkMovement(unit, 2);
                blink.SetPath(_cast_unit_hex, targetable_hex);
                unit.AddBehaviourToWork(blink);

                blink.OnEndBehaviour += OnEndOfMovement;

            }
            else
            {
                unit.Move(_cast_unit_hex, targetable_hex);
                unit.GetBehaviour<MovementBehaviour>().OnEndBehaviour += OnEndOfMovement;
            }

            int counter = 0;
            if (unit.class_type == ClassType.Light)
                counter = game.death_light;
            else
                counter = game.death_dark;

            if (counter <= 6)
            {
                unit.stats.damage = 2;
                unit.stats.max_health += 1;
                unit.stats.current_health += 1;
            }
            else if (counter <= 9)
            {
                unit.stats.damage = 4;
                unit.stats.max_health += 2;
                unit.stats.current_health += 2;
            }
            else
            {
                unit.stats.damage = 6;
                unit.stats.max_health += 3;
                unit.stats.current_health += 3;
            }
            unit.stats.attack_range = 1;
            unit.AddAttackBehaviour(new MeleeAttack(unit));
        }
        Exit();
    }

    private void OnEndOfMovement(Behaviour behaviour)
    {
        GameManager.Instance.game.game_events.OnChangeUnitData_Global?.Invoke(unit);
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();
        if (!king_activate_special)
        {
            Map map = GameManager.Instance.game.map;
            Hex grall_hex = map.GetHex(0, 0);
            Unit aliance = grall_hex.GetUnit();

            if ((grall_hex.IsWalkable() && map.HexesInRange(_unit_hex, ability_data.range).Contains(grall_hex)) || (aliance != null && aliance.class_type == unit.class_type && aliance.unit_type == UnitType.Tank))
                _available_moves.Add(grall_hex);
        }

        return _available_moves;
    }
}