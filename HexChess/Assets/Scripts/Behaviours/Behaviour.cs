using Newtonsoft.Json;
using System;
using UnityEngine;

public abstract class Behaviour
{
    public string sprite_path { get; set; }
    [JsonIgnore] public Sprite sprite { set; get; }
    [JsonRequired] protected Unit unit { get; set; }
    [JsonRequired] protected float time { get; set; }

    [JsonIgnore] public Action<Behaviour> OnStartBehaviour;
    [JsonIgnore] public Action<Behaviour> OnEndBehaviour;
    public virtual void Enter()
    {
        OnStartBehaviour?.Invoke(this);
        time = Time.time;
    }
    public abstract void Execute();
    public virtual void Exit()
    {
        OnEndBehaviour?.Invoke(this);
        unit.ChangeBehaviour();
    }

    public Behaviour() : base() { }
}
