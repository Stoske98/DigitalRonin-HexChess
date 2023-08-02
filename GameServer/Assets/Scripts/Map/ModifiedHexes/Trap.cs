public class Trap : HexModifier
{
    protected Unit cast_unit { set; get; }
    protected AbilityBehaviour ability { set; get; }
    public Trap(Unit _cast_unit, AbilityBehaviour _ability) : base() { cast_unit = _cast_unit; ability = _ability; }
    public override void Trigger(Unit _unit, Hex _hex)
    {
        if(_unit.class_type != cast_unit.class_type)
        {
            should_be_removed = true;
            _unit.RecieveDamage(new MagicDamage(cast_unit, ability.ability_data.amount));
            if (!_unit.IsDeath())
            {
                _unit.ccs.Add(new Stun(2));
                Behaviour behaviour = _unit.GetBehaviour<MovementBehaviour>();
                if (behaviour != null && behaviour is MovementBehaviour movement_behaviour && movement_behaviour.GetPath().Count != 0)
                {
                    for (int i = movement_behaviour.GetPath().Count - 1; i >= 0; i--)
                    {
                        if (movement_behaviour.GetPath()[i] == _hex)
                        {
                            if(_hex.IsWalkable())
                            {
                                _hex.PlaceObject(_unit);
                                NetworkManager.Instance.games[cast_unit.match_id].game_events.OnEndMovement_Global?.Invoke(_hex);
                            }
                            break;
                        }
                        else
                            movement_behaviour.GetPath().RemoveAt(i);
                    }

                }
                _unit.to_do_behaviours.Clear();
            }
        }
    }
}
