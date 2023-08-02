using Newtonsoft.Json;

public class KingPassive : PassiveAbility
{
    [JsonRequired] public bool is_activated;
    public KingPassive() : base() { }
    public KingPassive(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
    }
    public override void UnregisterEvents()
    {
    }

    public void ActivateGrall()
    {
        if(GameManager.Instance.game is ChallengeRoyaleGame game)
        {
            int counter = 0;
            foreach (Unit death_unit in GameManager.Instance.game.units)
            {
                if (death_unit.stats.current_health == 0 && death_unit.class_type == unit.class_type)
                    counter++;
            }

            //upgrade king
            UnityEngine.Debug.Log("Challenge royale actiavetd");
            game.Activate();

            is_activated = true;
        }
    }

}

