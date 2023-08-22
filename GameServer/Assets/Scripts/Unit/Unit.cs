using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : IActiveObject, ISubscribe, IDamageableObject
{
    public string id { get; set; }
    public string game_object_path { get; set; }
    public int level { get; set; }
    public Stats stats { get; set; }
    public ClassType class_type { get; set; }
    public UnitType unit_type { get; set; }
    public Visibility visibility { get; set; }
    public bool is_immune_to_magic { get; set; }
    public string sprite_path { get; set; }
    [JsonIgnore] public Sprite sprite { set; get; }
    [JsonIgnore] public int match_id { get; set; }
    [JsonConverter(typeof(CustomConverters.GameObjectConverter))] public GameObject game_object { get; set; }
    [JsonConverter(typeof(CustomConverters.BehaviourListConverter))] public List<Behaviour> behaviours { get; set; }
    [JsonConverter(typeof(CustomConverters.CCListConverter))] public List<CC> ccs { set; get; }
    public List<Level> levels { get; set; }
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
        levels = new List<Level>();
    }
    public Unit(Game _game, ClassType _class_type, UnitType _unit_type, string _game_object_path, string _sprite_path)
    {
        id = _game.random_seeds_generator.GetRandomIdsSeed();
        match_id = _game.match_id;
        level = 0;
        class_type = _class_type;
        unit_type = _unit_type;
        is_immune_to_magic = false;
        game_object_path = _game_object_path;
        sprite_path = _sprite_path;

        stats = new Stats();
        events = new UnitEvents();
        behaviours = new List<Behaviour>();
        to_do_behaviours = new Queue<Behaviour>();
        ccs = new List<CC>();
        visibility = Visibility.BOTH;

        game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(game_object_path));
        game_object.transform.SetParent(MapContainer.Instance.units_container);
        game_object.name = class_type.ToString() + "_" + unit_type.ToString();
        if (class_type == ClassType.Dark)
            game_object.transform.eulerAngles = new Vector3(0, 180, 0);

        sprite = Resources.Load<Sprite>(sprite_path);

    }
    public void Update()
    {
        if (to_do_behaviours.Count > 0)
            to_do_behaviours.Peek().Execute();
    }
    public void LevelUp()
    {
        levels[level].LevelUp(this);
    }
    public void AddLevels(List<Level> _levels)
    {
        levels = _levels;
    }
    public void AddBehaviourToWork(Behaviour _behaviour)
    {
        if (_behaviour != null)
        {
            if (to_do_behaviours.Count == 0)
            {
                to_do_behaviours.Enqueue(_behaviour);
                _behaviour.Enter();
            }
            else
                to_do_behaviours.Enqueue(_behaviour);
        }
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
        }
        else if (typeof(T) == typeof(Ability))
        {
            if (_code == KeyCode.S || _code == KeyCode.Q || _code == KeyCode.W || _code == KeyCode.E || _code == KeyCode.R)
                return GetAbilityByPosition(_code);
        }
        return default;
    }
    public void AddMovementBehaviour(MovementBehaviour movement_behaviour)
    {
        int counter = 0;
        foreach (var behaviour in behaviours)
        {
            if (behaviour is MovementBehaviour)
                break;

            counter++;
        }

        if (counter == behaviours.Count)
            behaviours.Add(movement_behaviour);
        else
            behaviours[counter] = movement_behaviour;
    }
    public void AddAttackBehaviour(AttackBehaviour attack_behaviour)
    {
        int counter = 0;
        foreach (var behaviour in behaviours)
        {
            if (behaviour is AttackBehaviour)
                break;

            counter++;
        }

        if (counter == behaviours.Count)
            behaviours.Add(attack_behaviour);
        else
            behaviours[counter] = attack_behaviour;
    }
    public void SwitchAbilityBehaviourWithNewOne(Behaviour behaviour, KeyCode key_code)
    {
        int counter = 0;
        int ability_position_counter = 0;
        int ability_position = AbilityPosition(key_code);

        if (ability_position > -1)
        {
            foreach (Behaviour _behaviour in behaviours)
            {
                if (_behaviour is AbilityBehaviour ability_behaviour)
                {
                    if (ability_position_counter == ability_position)
                        break;
                    else
                        ability_position_counter++;
                }
                counter++;
            }
        }
        if (behaviours[counter] is ISubscribe unsubscriber)
            unsubscriber.UnregisterEvents();

        if(behaviour is ISubscribe subscibers)
            subscibers.RegisterEvents();

        if (behaviours[counter] is Ability ability_old && behaviour is Ability ability_new)
        {
            if (ability_new.ability_data.max_cooldown <= ability_old.ability_data.current_cooldown)
                ability_new.ability_data.current_cooldown = ability_new.ability_data.max_cooldown;
            else
                ability_new.ability_data.current_cooldown = ability_old.ability_data.current_cooldown;
        }

        behaviours[counter] = behaviour;

    }
    private Behaviour GetAbilityByPosition(KeyCode key_code)
    {
        int counter = 0;
        int ability_position = AbilityPosition(key_code);

        if (ability_position > -1)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                if (behaviour is AbilityBehaviour ability_behaviour)
                {
                    if (counter == ability_position)
                        return ability_behaviour;
                    else
                        counter++;
                }
            }

        }
        return null;
    }

    private int AbilityPosition(KeyCode key_code)
    {
        int position = -1;

        if (key_code == KeyCode.S)
            position = 0; //special
        else if (key_code == KeyCode.Q)
            position = 1; //ability 1
        else if (key_code == KeyCode.W)
            position = 2; //ability 2
        else if (key_code == KeyCode.E)
            position = 3; //ability 3
        else if (key_code == KeyCode.R)
            position = 4; //ability 4

        return position;
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
    public void UseInstantAbility(InstantleAbility instant)
    {
        instant.SetAbility();
        //Change cooldown logic
        instant.ability_data.current_cooldown = instant.ability_data.max_cooldown;
        AddBehaviourToWork(instant);
    }
    public void UseSingleTargetableAbility(TargetableAbility targetable_ability, Hex hex)
    {
        ((ITargetableSingleHex)targetable_ability).SetAbility(hex);
        //Change cooldown logic
        targetable_ability.ability_data.current_cooldown = targetable_ability.ability_data.max_cooldown;
        AddBehaviourToWork(targetable_ability);

    }
    public void UseMultipleTargetableAbility(TargetableAbility targetable_ability, List<Hex> hexes)
    {
        ((ITargetMultipleHexes)targetable_ability).SetAbility(hexes);
        //Change cooldown logic
        targetable_ability.ability_data.current_cooldown = targetable_ability.ability_data.max_cooldown;
        AddBehaviourToWork(targetable_ability);
    }

    public void ChangeBehaviour()
    {
        if(to_do_behaviours.Count > 0)
            to_do_behaviours.Dequeue();

        if (to_do_behaviours.Count > 0)
            to_do_behaviours.Peek().Enter();
    }   
    public void ReceiveDamage(Damage damage)
    {
        events.OnBeforeReceivingDamage_Local?.Invoke(damage);

        Hex _unit_hex = NetworkManager.Instance.games[match_id].map.GetHex(this);
        if (damage is PhysicalDamage physical_damage)
        {
            if (physical_damage.miss)
            {
                Debug.Log("MISS");
                return;
            }

            if (physical_damage is CritDamage crit_damage)
                Debug.Log("CRITICAL");

            stats.current_health -= damage.amount;

            events.OnRecieveDamage_Local?.Invoke(_unit_hex);

            if (IsDead())
                Die(_unit_hex);
        }
        else
        {
            stats.current_health -= damage.amount;

            events.OnRecieveDamage_Local?.Invoke(_unit_hex);

            if (IsDead())
                Die(_unit_hex);

        }
    }
    public void Die(Hex hex)
    {
        stats.current_health = 0;
        hex.RemoveObject(this);
        to_do_behaviours.Clear(); 
        IObject.ObjectVisibility(this, Visibility.NONE);

        ChallengeRoyaleGame ch_game = NetworkManager.Instance.games[match_id] as ChallengeRoyaleGame;
        if(ch_game != null)
            ch_game.shard_controller.IncreaseShardsOnUnitDeath(class_type,unit_type);
    }
    public bool IsDead()
    {
       return stats.current_health <= 0;
    }

    public virtual void RegisterEvents()
    {
        foreach (Behaviour behaviour in behaviours.ToArray())
        {
            if (behaviour is ISubscribe subscriber)
                subscriber.RegisterEvents();
        }
    }
    public virtual void UnregisterEvents()
    {
        foreach (Behaviour behaviour in behaviours.ToArray())
        {
            if (behaviour is ISubscribe subscriber)
                subscriber.UnregisterEvents();
        }
    }

    public void UpdateCCsCooldown()
    {
        foreach (CC cc in ccs)
            cc.UpdateCooldown();

        ccs.RemoveAll(obj => obj.current_cooldown == 0);
    }

    public void UpdateAbilitiesCooldown()
    {
        foreach (Behaviour behaviour in behaviours)
            if(behaviour is Ability cooldown_ability)
                cooldown_ability.UpdateCooldown();
    }

}




