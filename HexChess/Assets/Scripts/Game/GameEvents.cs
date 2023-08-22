using System;

public class GameEvents
{
    public Action<Hex, Hex> OnStartMovement_Global;
    public Action<Hex> OnEndMovement_Global;
    public Action<ClassType> OnEndPlayerTurn_Global;
    public Action<ChallengeRoyaleGame> OnShardChanges_Global;
    public Action<ClassLevelController, UnitType> OnClassUpgraded_Global;
    public Action<Unit> OnUnitDeath_Global;
    public Action<Unit, Damage, Hex> OnReceiveDamage_Global;
    public Action<Unit> OnUseAbility_Global;
    public Action<Unit> OnUpdateCooldown_Global;


    public Action<Game> OnEndTurnDisplay;

}


