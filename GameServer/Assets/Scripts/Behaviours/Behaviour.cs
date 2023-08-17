using Newtonsoft.Json;
using System;

public abstract class Behaviour
{
    [JsonRequired] protected Unit unit { get; set; }
    [JsonRequired] protected float time { get; set; }

    [JsonIgnore] public Action<Behaviour> OnStartBehaviour;
    [JsonIgnore] public Action<Behaviour> OnEndBehaviour;
    public virtual void Enter()
    {
        OnStartBehaviour?.Invoke(this);
    }
    public abstract void Execute();
    public virtual void Exit()
    {
        OnEndBehaviour?.Invoke(this);
        unit.ChangeBehaviour();
    }

    public Behaviour() : base() { }
}
