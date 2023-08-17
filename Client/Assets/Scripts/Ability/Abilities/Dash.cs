using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dash : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    [JsonRequired] private bool upgraded { get; set; }
    [JsonIgnore] private Direction face_direction { get; set; }
    [JsonIgnore] private List<Hex> hexes_in_direction { get; set; }
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Dash() : base() { hexes_in_direction = new List<Hex>(); }
    public Dash(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { hexes_in_direction = new List<Hex>(); upgraded = false; }

    public override void Execute()
    {
        Hex _cast_unit_hex = GameManager.Instance.game.map.GetHex(unit);

        KnightMovement movement = new KnightMovement(unit);
        movement.SetPath(_cast_unit_hex, targetable_hex);
        unit.AddBehaviourToWork(movement);

        if (upgraded)
        {
            Map map = GameManager.Instance.game.map;

            face_direction = map.CoordinatesToDirection(map.
                TransformCoordinatesToUnitCoordinates(new Vector2Int(targetable_hex.coordinates.x - _cast_unit_hex.coordinates.x, targetable_hex.coordinates.y - _cast_unit_hex.coordinates.y)));
            unit.events.OnEndMovement_Local += OnEndMovementSlash;
        }

        foreach (var hex in hexes_in_direction)
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
        }
        targetable_hex = null;
        hexes_in_direction.Clear();
        Exit();
    }

    private void OnEndMovementSlash(Hex hex)
    {
        unit.events.OnEndMovement_Local -= OnEndMovementSlash;
        if (!unit.IsDead())
        {
            Map map = GameManager.Instance.game.map;

            Vector2Int face_direction_coordinates = map.DirectionToCoordinates(face_direction);
            Vector2Int face_hex_coordinates = face_direction_coordinates + hex.coordinates;

            List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
            foreach (var coordinate in map.neighbors_vectors)
                unit_neighbors_coordinates.Add(hex.coordinates + coordinate);

            List<Vector2Int> face_hex_neigbors = new List<Vector2Int>();
            foreach (var coordinate in map.neighbors_vectors)
                face_hex_neigbors.Add(face_hex_coordinates + coordinate);

            List<Vector2Int> same_neighbor_coordinates = unit_neighbors_coordinates.Intersect(face_hex_neigbors).ToList();

            DealDamageToEnemies(map, face_hex_coordinates);

            foreach (var coordinate in same_neighbor_coordinates)
                DealDamageToEnemies(map, coordinate);
        }
    }
    private void DealDamageToEnemies(Map map, Vector2Int coordinates)
    {
        Hex hex = map.GetHex(coordinates.x, coordinates.y);

        if (hex != null)
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && enemy.class_type != unit.class_type)
            {
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));
            }
        }
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = GameManager.Instance.game;

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
        if (ability_data.range == hexes.Count && hexes[^1] != null && hexes[^1].IsWalkable())
            return hexes;

        hexes.Clear();
        return hexes;
    }
    public void SetAbility(Hex _targetable_hex)
    {
        Map map = GameManager.Instance.game.map;
        Hex unit_hex = map.GetHex(unit);
        Vector2Int _target_hex_coordinate_direacion = map.TransformCoordinatesToUnitCoordinates(new Vector2Int(_targetable_hex.coordinates.x - unit_hex.coordinates.x, _targetable_hex.coordinates.y - unit_hex.coordinates.y));
        Vector2Int _target_hex_position = _target_hex_coordinate_direacion + unit_hex.coordinates;

        for (int i = 0; i < ability_data.range - 1; i++)
        {
            hexes_in_direction.Add(map.GetHex(_target_hex_position.x, _target_hex_position.y));
            _target_hex_position += _target_hex_coordinate_direacion;
        }

        targetable_hex = map.GetHex(_target_hex_position.x, _target_hex_position.y);
    }
    public void Upgrade()
    {
        upgraded = true;
    }
}
