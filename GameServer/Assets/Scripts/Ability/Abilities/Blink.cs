using Newtonsoft.Json;
using System.Collections.Generic;

public class Blink : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    [JsonRequired] private bool upgraded { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }    
  
    public Blink() : base() {  }
    public Blink(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { upgraded = false; }

    public override void Execute()
    {
        Hex _unit_hex = NetworkManager.Instance.games[unit.match_id].map.GetHex(unit);

        TeleportMovement teleport_movement = new TeleportMovement(unit);
        teleport_movement.SetPath(_unit_hex, targetable_hex);
        unit.AddBehaviourToWork(teleport_movement);

        if(upgraded)
            unit.events.OnEndMovement_Local += OnEndOfMovementJuggernaut;

        targetable_hex = null;
        Exit();
    }

    private void OnEndOfMovementJuggernaut(Hex hex)
    {
        unit.events.OnEndMovement_Local -= OnEndOfMovementJuggernaut;
        if(!unit.IsDead())
        {
            foreach (Hex _hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(hex, ability_data.range))
            {
                Unit enemy = _hex.GetUnit();
                if (enemy != null && enemy.class_type != unit.class_type)
                    enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

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

        return _ability_moves;
    }

    public void Upgrade()
    {
        upgraded = true;
    }
}