using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public StatsUpdate update_stats { get; set; }
    [JsonConverter(typeof(CustomConverters.BehaviourListConverter))] public List<Behaviour> behaviours_to_add { get; set; }
    [JsonConverter(typeof(CustomConverters.DictionaryConverter))] public Dictionary<KeyCode, Behaviour> behaviour_to_switch { get; set; }

    public Level()
    {
        behaviours_to_add = new List<Behaviour>();
        behaviour_to_switch = new Dictionary<KeyCode, Behaviour>();
    }
    public void LevelUp(Unit unit)
    {
        unit.level++;
        //update stats
        unit.stats.max_health += update_stats.increase_max_health;
        unit.stats.current_health += update_stats.increase_max_health;
        unit.stats.damage += update_stats.increase_damage;
        unit.stats.attack_range += update_stats.increase_attack_range;
        unit.stats.attack_speed += update_stats.increase_attack_speed;

        //update abilities
        foreach (var behaviour in unit.behaviours)
            if (behaviour is IUpgradable upgradable_behaviour)
                upgradable_behaviour.Upgrade();

        //switch ability behaviour with new one
        foreach (KeyCode key_code in behaviour_to_switch.Keys)
            if (key_code == KeyCode.S || key_code == KeyCode.Q || key_code == KeyCode.W || key_code == KeyCode.E || key_code == KeyCode.R)
                unit.SwitchAbilityBehaviourWithNewOne(behaviour_to_switch[key_code], key_code);

        //add new behaviour
        foreach (var behaviour in behaviours_to_add)
        {
            if (behaviour is MovementBehaviour movement_behaviour)
            {
                //remove exist movement behaviour and add new one
                unit.AddMovementBehaviour(movement_behaviour);

            }
            else if (behaviour is AttackBehaviour attack_behaviour)
            {
                //remove exist attack behaviour and add new one
                unit.AddAttackBehaviour(attack_behaviour);
            }
            else
            {
                // add to behaviour
                unit.behaviours.Add(behaviour);

                if (behaviour is ISubscribe subsciber)
                    subsciber.RegisterEvents();
            }
        }

    }
}




