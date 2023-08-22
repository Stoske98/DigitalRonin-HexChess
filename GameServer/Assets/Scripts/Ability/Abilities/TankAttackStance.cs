using System.Collections.Generic;
using UnityEngine;

public class TankAttackStance : TargetableAbility, ITargetableSingleHex
{
    public Hex targetable_hex { get; set; }
    public TankDefenceStance tank_deffence_stance { get; set; }
    public TankAttackStance() : base() {  }
    public TankAttackStance(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        tank_deffence_stance = new TankDefenceStance(unit, new AbilityData()
        {
            range = 1,
            amount = 3,
            max_cooldown = 2,
            current_cooldown = 0
        }, "UI/Unit/Tank/Special/defence_stance");

        unit.behaviours.Add(tank_deffence_stance);

        if (tank_deffence_stance is ISubscribe subsciber)
            subsciber.RegisterEvents();
    }

    public override void Execute()
    {
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        Hex unit_hex = map.GetHex(unit);

        Unit enemy = map.GetHex(targetable_hex.coordinates.x, targetable_hex.coordinates.y)?.GetUnit();

        Vector2Int direction = targetable_hex.coordinates - unit_hex.coordinates;
        Vector2Int unit_direction = map.TransformCoordinatesToUnitCoordinates(direction);

        if (enemy == null) 
        {

            Hex enemy_hex = map.GetHex(unit_hex.coordinates.x + unit_direction.x, unit_hex.coordinates.y + unit_direction.y);
            enemy = enemy_hex.GetUnit();
            RemoveAndMoveEnemy(enemy, enemy_hex, targetable_hex);

            unit.Move(unit_hex, enemy_hex);
        }
        else
        {
            Hex hex_in_direction = map.GetHex(targetable_hex.coordinates.x + unit_direction.x, targetable_hex.coordinates.y + unit_direction.y);
            if (hex_in_direction != null && hex_in_direction.IsWalkable())
            {
                RemoveAndMoveEnemy(enemy, targetable_hex, hex_in_direction);
                unit.Move(unit_hex, targetable_hex);
            }
            else
                enemy.ccs.Add(new Stun(ability_data.cc));

        }

        Exit();
    }
    private void RemoveAndMoveEnemy(Unit enemy, Hex start_hex, Hex desired_hex)
    {
        NormalMovement normal_movement = new NormalMovement(enemy);
        normal_movement.SetPath(start_hex, desired_hex);
        enemy.AddBehaviourToWork(normal_movement);
        start_hex.RemoveObject(enemy);
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = NetworkManager.Instance.games[unit.match_id];

        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UP, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.DOWN, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_LEFT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.LOWER_RIGHT, _unit_hex, ability_data.range)));
        _ability_moves.AddRange(AvailableMovesInDirection(game.GetHexesInDirection(Direction.UPPER_RIGHT, _unit_hex, ability_data.range)));

        return _ability_moves;
    }

    private List<Hex> AvailableMovesInDirection(List<Hex> hexes)
    {
        if(hexes.Count > 0)
        {
            Unit enemy = hexes[0].GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
            {
                if(hexes.Count == 2 && !hexes[^1].IsWalkable())
                    hexes.RemoveAt(hexes.Count - 1);
            }else 
                hexes.Clear();
        }
        return hexes;
    }
}
