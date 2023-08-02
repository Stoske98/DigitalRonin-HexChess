public class Grall : HexModifier
{
    public override void Trigger(Unit _unit, Hex _hex)
    {
        if(_hex.GetUnit() != null && _hex.GetUnit() == _unit )
        {
            foreach (Behaviour behaviour in _unit.behaviours)
            {
                if(behaviour is KingPassive king_passive)
                {
                    if (king_passive.is_activated)
                        return;

                    king_passive.ActivateGrall();
                }
            }
        }
    }
}