using System;

public class GameEvents
{
    public Action<Hex, Hex> OnStartMovement_Global;
    public Action<Hex> OnEndMovement_Global;
   
    public Action OnEndTurn;

}


