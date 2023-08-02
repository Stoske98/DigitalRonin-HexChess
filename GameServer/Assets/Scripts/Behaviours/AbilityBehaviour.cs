using Newtonsoft.Json;

public abstract class AbilityBehaviour : Behaviour
{
    public AbilityData ability_data { get; set; }
    public AbilityBehaviour() : base() { }
    public AbilityBehaviour(Unit _unit, AbilityData _ability_data)
    { 
        unit = _unit; 
        ability_data = _ability_data;
    }
    public override void Enter()
    {
    }
    public override void Exit()
    {
        base.Exit();
    }
}

