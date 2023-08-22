using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanSpecial : PassiveAbility
{
    [JsonRequired] private Direction face_direction { get; set; }
    public SwordsmanSpecial() : base() {  }
    public SwordsmanSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path ,Direction _direction) : base(_unit, _ability_data, _sprite_path) 
    {
        face_direction = _direction;

    }
    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovementCalculateNewUnitDirection;
        NetworkManager.Instance.games[unit.match_id].game_events.OnEndMovement_Global += OnEndOfMovementGlobalUnit;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovementCalculateNewUnitDirection;
        NetworkManager.Instance.games[unit.match_id].game_events.OnEndMovement_Global -= OnEndOfMovementGlobalUnit;
    }

    private void OnStartMovementCalculateNewUnitDirection(Hex start_hex, Hex desired_hex)
    {
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        face_direction = map.CoordinatesToDirection(map.
            TransformCoordinatesToUnitCoordinates(new Vector2Int(desired_hex.coordinates.x - start_hex.coordinates.x, desired_hex.coordinates.y - start_hex.coordinates.y)));
    }

    private void OnEndOfMovementGlobalUnit(Hex hex)
    {
        Unit global_unit = hex.GetUnit();
        
        if (global_unit != null && global_unit.class_type != unit.class_type && !global_unit.IsDead()) 
        {
            Game game = NetworkManager.Instance.games[unit.match_id];
            Hex unit_hex = game.map.GetHex(unit);
            if(unit_hex != null)
            {
                Vector2Int face_direction_coordinates = game.map.DirectionToCoordinates(face_direction);
                Vector2Int face_hex_coordinates = face_direction_coordinates + unit_hex.coordinates;

                List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    unit_neighbors_coordinates.Add(unit_hex.coordinates + coordinate);

                List<Vector2Int> face_hex_neigbors = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    face_hex_neigbors.Add(face_hex_coordinates + coordinate);

                if (face_hex_coordinates == hex.coordinates || (unit_neighbors_coordinates.Contains(hex.coordinates) && face_hex_neigbors.Contains(hex.coordinates)))
                    unit.Attack(global_unit);
            }
        }
    }

}
