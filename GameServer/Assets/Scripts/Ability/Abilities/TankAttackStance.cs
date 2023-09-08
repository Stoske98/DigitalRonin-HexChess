using System.Collections.Generic;
using UnityEngine;

public class TankAttackStance : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    public Hex targetable_hex { get; set; }
    private Unit enemy { get; set; }
    private Hex enemy_hex { get; set; }
    private Hex front_enemy_hex { get; set; }
    private Hex behind_enemy_hex { get; set; }
    public TankAttackStance() : base() {  }
    public TankAttackStance(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) { }

    public override void Execute()
    {
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        Hex unit_hex = map.GetHex(unit);

        Vector2Int direction = map.TransformCoordinatesToUnitCoordinates(targetable_hex.coordinates - unit_hex.coordinates);

        for (int i = 1; i <= ability_data.range; i++)
        {
            Hex hex = map.GetHex(unit_hex.coordinates.x + direction.x * i, unit_hex.coordinates.y + direction.y * i);
            if (hex != null)
            {
                enemy = hex.GetUnit();
                if (enemy != null)
                {
                    enemy_hex = hex;
                    behind_enemy_hex = map.GetHex(enemy_hex.coordinates.x + direction.x, enemy_hex.coordinates.y + direction.y);
                    front_enemy_hex = map.GetHex(enemy_hex.coordinates.x - direction.x, enemy_hex.coordinates.y - direction.y);

                    if (behind_enemy_hex != null && behind_enemy_hex.IsWalkable())
                    {
                        enemy_hex.RemoveObject(enemy);
                        unit.GetBehaviour<MovementBehaviour>().OnEndBehaviour += OnEndOfTankCharge;
                        unit.Move(unit_hex, enemy_hex);
                    }
                    else if (front_enemy_hex != unit_hex)
                    {
                        unit.Move(unit_hex, front_enemy_hex);
                        unit.GetBehaviour<MovementBehaviour>().OnEndBehaviour += OnEndOfTankCharge;
                    }
                    else
                    {
                        enemy.ccs.Add(new Stun(unit, enemy, ability_data.cc));

                        enemy = null;
                        enemy_hex = null;
                        front_enemy_hex = null;
                        behind_enemy_hex = null;
                    }
                    break;
                }
            }
        }
        Exit();
    }

    private void OnEndOfTankCharge(Behaviour behaviour)
    {
        behaviour.OnEndBehaviour -= OnEndOfTankCharge;
        if (!unit.IsDead())
        {
            Map map = NetworkManager.Instance.games[unit.match_id].map;
            Hex unit_hex = map.GetHex(unit);
            if (unit_hex != null && !Stun.IsStuned(unit))
            {
                if (unit_hex == front_enemy_hex)
                {
                    enemy.ccs.Add(new Stun(unit, enemy, ability_data.cc));
                }
                else if (unit_hex == enemy_hex)
                {
                    NormalMovement normal_movement = new NormalMovement(enemy);
                    normal_movement.SetPath(enemy_hex, behind_enemy_hex);
                    enemy.AddBehaviourToWork(normal_movement);
                }
            }
        }

        enemy = null;
        enemy_hex = null;
        front_enemy_hex = null;
        behind_enemy_hex = null;
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
        int count = 0;

        for (int i = 0; i < hexes.Count; i++)
        {
            Unit enemy = hexes[i].GetUnit();
            if (enemy != null)
            {
                if (enemy.class_type == unit.class_type)
                    return new List<Hex>();
                else
                {
                    count = i + 1;
                    break;
                }
            }
        }

        if (count != 0)
            hexes = hexes.GetRange(0, count);
        else
            hexes.Clear();

        return hexes;
    }

    public void Upgrade()
    {
        ability_data.range += 1;
        unit.GetBehaviour<MovementBehaviour>().range += 1;
    }
}
