using Newtonsoft.Json;
using System.Collections.Generic;

public class Blink : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Blink() : base() {  }
    public Blink(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {
        Hex _unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(unit);

        BlinkMovement blink = new BlinkMovement(unit,2);
        blink.SetPath(_unit_hex, targetable_hex);
        unit.AddBehaviourToWork(blink);

        if (unit.level == 3)
            blink.OnEndBehaviour += OnEndOfMovementJuggernaut;
           // unit.events.OnEndMovement_Local += OnEndOfMovementJuggernaut;

        targetable_hex = null;
        Exit();
    }

    private void OnEndOfMovementJuggernaut(Behaviour behaviour)
    {
        behaviour.OnEndBehaviour -= OnEndOfMovementJuggernaut;
        if(!unit.IsDead())
        {
            Map map = NetworkManager.Instance.games[unit.match_id].map;
            Hex unit_hex = map.GetHex(unit);
            if(unit_hex != null)
            {
                foreach (Hex _hex in map.HexesInRange(unit_hex, ability_data.range))
                {
                    Unit enemy = _hex.GetUnit();
                    if (enemy != null && enemy.class_type != unit.class_type)
                        enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

                }
            }
        }
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Map map = NetworkManager.Instance.games[unit.match_id].map;
        foreach (var diagonal_coordinate in map.diagonals_neighbors_vectors)
        {
            Hex _desired_hex = map.GetHex(_unit_hex.coordinates.x + diagonal_coordinate.x, _unit_hex.coordinates.y + diagonal_coordinate.y);
            if(_desired_hex != null && _desired_hex.IsWalkable())
                _ability_moves.Add(_desired_hex);
        }

        unit.events.OnGetAbilityMoves_Local?.Invoke(_unit_hex, _ability_moves);
        return _ability_moves;
    }

    public void Upgrade()
    {
        ability_data.amount = 1;
        ability_data.max_cooldown = 3;
    }
}