using Newtonsoft.Json;

public abstract class Behaviour
{
    [JsonRequired] protected Unit unit { get; set; }
    [JsonRequired] protected float time { get; set; }
    public abstract void Enter();
    public abstract void Execute();
    public virtual void Exit()
    {
        unit.ChangeBehaviour();
    }

    public Behaviour() : base() { }
}