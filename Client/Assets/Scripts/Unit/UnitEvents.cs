using System;
using System.Collections.Generic;

public class UnitEvents
{
    public Action<Hex, Hex> OnStartMovement_Local;

    public Action<Hex> OnEndMovement_Local;

    public Action<Hex> OnRecieveDamage_Local;

    public Action<Hex, List<Hex>> OnGetAttackMoves_Local;

    public Action<Damage> OnBeforeReceivingDamage_Local;

    public Func<Damage, Damage> OnStartAttack_local;


}




