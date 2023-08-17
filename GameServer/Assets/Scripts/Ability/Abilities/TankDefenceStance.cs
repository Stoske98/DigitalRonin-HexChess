using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TankDefenceStance : TargetableAbility, ITargetableSingleHex
{
    public Hex targetable_hex { get; set; }

    public List<Unit> stones;
    public TankDefenceStance() : base() { stones = new List<Unit>(); }
    public TankDefenceStance(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) 
    {
        Game game = NetworkManager.Instance.games[unit.match_id];

        stones = new List<Unit>();
        for (int i = 0; i < 3; i++)
          stones.Add(Spawner.CreateStone(game, unit.class_type));
    }

    public override void Execute()
    {
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        Hex unit_hex = map.GetHex(unit);

        List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
        foreach (var coordinate in map.neighbors_vectors)
            unit_neighbors_coordinates.Add(unit_hex.coordinates + coordinate);

        List<Vector2Int> stone_hex_neigbors = new List<Vector2Int>();
        foreach (var coordinate in map.neighbors_vectors)
            stone_hex_neigbors.Add(targetable_hex.coordinates + coordinate);

        List<Vector2Int> same_neighbor_coordinates = unit_neighbors_coordinates.Intersect(stone_hex_neigbors).ToList();

        for (int i = 0;i < same_neighbor_coordinates.Count;i++)
            PlaceStone(stones[i], map, same_neighbor_coordinates[i].x, same_neighbor_coordinates[i].y);

        PlaceStone(stones[2], map, targetable_hex.coordinates.x, targetable_hex.coordinates.y);

        Exit();
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();
        Map map = NetworkManager.Instance.games[unit.match_id].map;

        foreach (Hex hex in NetworkManager.Instance.games[unit.match_id].map.HexesInRange(_unit_hex, ability_data.range))
        {
            if(hex != null && hex.IsWalkable())
                _ability_moves.AddRange(CheckInDirection(_unit_hex, hex, map));
        }
        
        return _ability_moves;
    }
    public List<Hex> CheckInDirection(Hex center_hex, Hex desired_hex, Map map)
    {
        List<Hex> _ability_moves = new List<Hex>();

        List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
        foreach (var coordinate in map.neighbors_vectors)
            unit_neighbors_coordinates.Add(center_hex.coordinates + coordinate);

        List<Vector2Int> stone_hex_neigbors = new List<Vector2Int>();
        foreach (var coordinate in map.neighbors_vectors)
            stone_hex_neigbors.Add(desired_hex.coordinates + coordinate);

        List<Vector2Int> same_neighbor_coordinates = unit_neighbors_coordinates.Intersect(stone_hex_neigbors).ToList();

        if (same_neighbor_coordinates.Count == 2)
        {
            _ability_moves.Add(desired_hex);
            foreach (var neighbor in same_neighbor_coordinates)
            {
                Hex hex = map.GetHex(neighbor.x, neighbor.y);
                if (hex == null || !hex.IsWalkable())
                {
                    _ability_moves.Clear();
                    break;
                }
            }
        }

        return _ability_moves;
    }
    private void PlaceStone(Unit stone, Map map, int column, int row)
    {
        map.GetHex(stone)?.RemoveObject(stone);
        map.PlaceObject(stone, column, row);
        stone.stats.current_health = stone.stats.max_health;
    }

}
