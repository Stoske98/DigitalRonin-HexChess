using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


public class UpdateManager
{
    public int max_level = 3;
    public int level { set; get; }
    public string description { get; set; }
    public Unit unit { get; set; }
    public void LevelUp()
    {
        if(level < max_level)
        {
            level++;
            foreach (var behaviour in unit.behaviours)
                if (behaviour is IUpgradable upgradable_behaviour)
                    upgradable_behaviour.Upgrade();
        }
    }
}
public class Unit : ISubscribe, IObject
{
    public string id { get; set; }
    public Stats stats { get; set; }
    public ClassType class_type { get; set; }
    public UnitType unit_type { get; set; }
    [JsonIgnore] public int match_id { get; set; }
    [JsonConverter(typeof(CustomConverters.UnitGameObjectConverter))] public GameObject game_object { get; set; }
    [JsonConverter(typeof(CustomConverters.BehaviourListConverter))] public List<Behaviour> behaviours { get; set; }
    [JsonConverter(typeof(CustomConverters.CCListConverter))] public List<CC> ccs { set; get; }

    //JSON IGNORE
    [JsonIgnore] public Queue<Behaviour> to_do_behaviours { set; get; }
    [JsonIgnore] public Quaternion target_rotation { set; get; }
    [JsonIgnore] public UnitEvents events { set; get; }

    [JsonConstructor]
    public Unit() 
    {
        behaviours = new List<Behaviour>();
        to_do_behaviours = new Queue<Behaviour>();
        ccs = new List<CC>();
        events = new UnitEvents();
    }

    //CHANGE CONSTRUCTOR
    public Unit(ClassType _class_type, UnitType _unit_type, Stats _stats)
    {
        id = Guid.NewGuid().ToString();
        stats = _stats;
        class_type = _class_type;
        unit_type = _unit_type;

        events = new UnitEvents();
        behaviours = new List<Behaviour>();
        to_do_behaviours = new Queue<Behaviour>();
        ccs = new List<CC>();

        //////////////////////////////////////////
        if (NetworkManager.Instance.gameobject_visibility)
        {
            game_object = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            game_object.name = class_type.ToString() + "_" + unit_type.ToString();
            game_object.GetComponent<Collider>().enabled = false;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (class_type == ClassType.LIGHT)
                go.GetComponent<Renderer>().material.color = Color.white;
            else
                go.GetComponent<Renderer>().material.color = Color.black;
            go.transform.SetParent(game_object.transform);

            go.GetComponent<Collider>().enabled = false;
            go.transform.localScale = new Vector3(1.25f, 1, 1.25f);
        }else
            game_object = new GameObject(class_type.ToString() + "_" + unit_type.ToString());

    }
    public void Update()
    {
        if (to_do_behaviours.Count > 0)
            to_do_behaviours.Peek().Execute();
    }
    public void AddBehaviourToWork(Behaviour _behaviour)
    {
        if (_behaviour != null)
            to_do_behaviours.Enqueue(_behaviour);
    }
    public void AddBehaviour(Behaviour _behaviour)
    {
        if (_behaviour != null)
            behaviours.Add(_behaviour);

    }
    public bool IsWork()
    {
        return to_do_behaviours.Count > 0 ? true : false;
    }
    public Behaviour GetBehaviour<T> (KeyCode _code = KeyCode.None)
    {
        if (_code == KeyCode.None)
        {
            if (typeof(T) == typeof(MovementBehaviour))
            {
                foreach (Behaviour behaviour in behaviours)
                {
                    if (behaviour is MovementBehaviour movement_behaviour)
                    {
                        //TO DO : chekc movmeent behaviour priority then return movement behaviour
                        return movement_behaviour;

                    }
                }
            }
            else if (typeof(T) == typeof(AttackBehaviour))
            {
                foreach (Behaviour behaviour in behaviours)
                {
                    if (behaviour is AttackBehaviour attack_behaviour)
                    {
                        //TO DO : chekc attack behaviour priority then return movement behaviour
                        return attack_behaviour;

                    }
                }
            }
        }else if (typeof(T) == typeof(Ability))
        {
            if (_code == KeyCode.S)
                return GetAbilityByPosition(0); //special
            if (_code == KeyCode.Q)
                return GetAbilityByPosition(1); //ability 1
            if (_code == KeyCode.W)
                return GetAbilityByPosition(2); //ability 2
            if (_code == KeyCode.E)
                return GetAbilityByPosition(3); //ability 3
            if (_code == KeyCode.R)
                return GetAbilityByPosition(4); //ability 4
        }
        return default;
    }
    private Behaviour GetAbilityByPosition(int _pos)
    {
        int counter = 0;
        foreach (Behaviour behaviour in behaviours)
        {
            if (behaviour is AbilityBehaviour ability_behaviour)
            {
                if (counter == _pos)
                    return ability_behaviour;
                else
                    counter++;
            }
        }
        return null;
    }
    public void Move(Hex _unit_hex, Hex _desired_hex)
    {
        Behaviour behaviour = GetBehaviour<MovementBehaviour>();
        if (behaviour != null && behaviour is MovementBehaviour movement_behaviour)
        {
            movement_behaviour.SetPath(_unit_hex, _desired_hex);
            AddBehaviourToWork(movement_behaviour);
        }
        else Debug.Log("ERROR: MOVEMENT NULL !!!");
    }
    public void Attack(Unit _target)
    {
        Behaviour behaviour = GetBehaviour<AttackBehaviour>();
        if(behaviour != null && behaviour is AttackBehaviour attack_behaviour)
        {
            attack_behaviour.SetAttack(_target);
            AddBehaviourToWork(attack_behaviour);
        }
        else Debug.Log("ERROR: ATTACK NULL !!!");
    }

    public void UseAbility(Ability ability, Hex targetable_hex = null)
    {
        if(behaviours.Contains(ability))
        {
            if (ability is TargetableAbility targetable)
            {
                if (targetable_hex != null)
                {
                    targetable.SetAbility(targetable_hex);
                    AddBehaviourToWork(targetable);
                }
            }
            else if (ability is InstantleAbility instant)
            {
                instant.SetAbility();
                AddBehaviourToWork(instant);
            }
        }
        else Debug.Log("ERROR: ABILITY NULL !!!");
    }
    public void ChangeBehaviour()
    {
        if(to_do_behaviours.Count > 0)
            to_do_behaviours.Dequeue();

        if (to_do_behaviours.Count > 0)
            to_do_behaviours.Peek().Enter();
    }
    public virtual void RecieveDamage(Damage damage)
    {
        stats.current_health -= damage.amount;
        Hex _unit_hex = NetworkManager.Instance.games[match_id].GetHex(this);

        if (stats.current_health <= 0)
        {
            stats.current_health = 0;
            _unit_hex?.RemoveUnit();

            //--should be removed--
            game_object.SetActive(false);
        }

        events.OnRecieveDamage_Local?.Invoke(_unit_hex);
    }

    public bool IsDeath()
    {
       return stats.current_health == 0;
    }

    public virtual void RegisterEvents()
    {

        foreach (Behaviour behaviour in behaviours.ToArray())
        {
            if(behaviour is PassiveAbility passive)
                  passive.RegisterEvents();
        }
    }
    public virtual void UnregisterEvents()
    {
        foreach (Behaviour behaviour in behaviours.ToArray())
        {
            if (behaviour is PassiveAbility passive)
                passive.UnregisterEvents();
        }
    }

    public void UpdateCCsCooldown()
    {
        foreach (CC cc in ccs)
            cc.UpdateCooldown();
    }

    public void UpdateAbilitiesCooldown()
    {
        // ?
        foreach (Behaviour behaviour in behaviours)
            if(behaviour is ICooldown cooldown_ability)
                cooldown_ability.UpdateCooldown();
    }

    public void Info(Hex _hex)
    {
        Debug.Log("---------------------------------------------------------------------------");
        Debug.Log(GetType().ToString() + "\nClass [" +class_type.ToString() + "]\nHex ["+ _hex .coordinates.x+ "][" + _hex.coordinates.y + "] " + "\nHP: " + stats.current_health);
        Debug.Log("---------------------------------------------------------------------------");
    }

}
public class UnitEvents
{
    public Action<Hex, Hex> OnStartMovement_Local;
    public Action<Hex> OnRecieveDamage_Local;
}




