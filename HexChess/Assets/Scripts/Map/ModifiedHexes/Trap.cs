using UnityEngine;
using Newtonsoft.Json;

public class Trap : HexModifier
{
    [JsonRequired] protected Unit cast_unit { set; get; }
    [JsonRequired] protected AbilityBehaviour ability { set; get; }
    [JsonConstructor]
    public Trap() : base()  {}
    public Trap(Unit _cast_unit, AbilityBehaviour _ability, string _game_object_path) : base() 
    {
        id = GameManager.Instance.game.random_seeds_generator.GetRandomIdSeed();
        cast_unit = _cast_unit;
        ability = _ability;
        game_object_path = _game_object_path;

        game_object = Object.Instantiate(Resources.Load<GameObject>(game_object_path));
    }
    public override void Trigger(Unit _unit, Hex _hex)
    {
        MovementBehaviour movement_behaviour = _unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
        if (_unit.class_type != cast_unit.class_type && movement_behaviour != null && !(movement_behaviour is FlyingMovement))
        {
            should_be_removed = true;
            _unit.ReceiveDamage(new MagicDamage(cast_unit, ability.ability_data.amount));

            if (!_unit.IsDead())
                _unit.ccs.Add(new Stun(ability.ability_data.cc));

            movement_behaviour.GetPath().Clear();
        }
    }

    public override void Update()
    {

    }
}
