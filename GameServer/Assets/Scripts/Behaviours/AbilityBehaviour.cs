using Newtonsoft.Json;
using UnityEngine;

public abstract class AbilityBehaviour : Behaviour
{
    public AbilityData ability_data { get; set; }
    public AbilityBehaviour() : base() { }
    public AbilityBehaviour(Unit _unit, AbilityData _ability_data, string _sprite_path)
    { 
        unit = _unit; 
        ability_data = _ability_data;
        sprite_path = _sprite_path;
        sprite = Resources.Load<Sprite>(sprite_path);
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
}

