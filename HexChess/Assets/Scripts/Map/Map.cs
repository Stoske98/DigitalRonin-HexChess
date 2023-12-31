﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Map
{
    [JsonRequired] protected int columns { get; set; }
    [JsonRequired] protected int rows { get; set; }
    [JsonRequired] protected float offset { get; set; }
    [JsonRequired] protected float height_of_hex { get; set; }
    public List<Hex> hexes { get; set; }

    [JsonIgnore]
    public readonly List<Vector2Int> neighbors_vectors = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(1, -1), new Vector2Int(-1, 0),
                                                                                new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -1) };

    [JsonIgnore]
    public readonly List<Vector2Int> diagonals_neighbors_vectors = new List<Vector2Int>() { new Vector2Int(2, -1), new Vector2Int(1, -2), new Vector2Int(-1, -1),
                                                                                        new Vector2Int(-2, 1), new Vector2Int(-1, 2), new Vector2Int(1, 1) };
    [JsonConstructor]
    public Map() 
    {
        hexes = new List<Hex>();
    }
    public Map(int _columns, int _rows, float _offset, float _height_of_hex)
    {
        columns = _columns;
        rows = _rows;
        offset = _offset;
        height_of_hex = _height_of_hex;
        hexes = new List<Hex>();
    }

    public abstract void CreateMap();
    public abstract void SpawnUnits(Game _game);

    public Hex GetHex(int column, int row)
    {
       
        foreach (Hex hex in hexes)
            if (hex.coordinates.x == column && hex.coordinates.y == row)
                return hex;

        return null;
    }
    public Hex GetHex(IObject obj)
    {
        foreach (Hex hex in hexes)
            foreach (IObject _ogj in hex.objects)
                if (_ogj == obj)
                    return hex;

        return null;
    }

    public List<Hex> HexesInRange(Hex hex, int range)
    {
        List<Hex> hices = new List<Hex>();

        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                Vector2Int neighbor_offset = new Vector2Int(q, r);
                Hex neighbor_in_range = GetHex(hex.coordinates.x + neighbor_offset.x, hex.coordinates.y + neighbor_offset.y);
                if (neighbor_in_range != null)
                    hices.Add(neighbor_in_range);
            }
        }
        return hices;
    }
    public bool PlaceObject(IObject obj, int column, int row)
    {
        foreach (Hex hex in hexes)
            if (hex.coordinates.x == column && hex.coordinates.y == row && hex.IsWalkable())
            {
                hex.PlaceObject(obj);
                obj.game_object.transform.position = hex.game_object.transform.position;
                obj.game_object.SetActive(true);
                return true;
            }
        return false;
    }

   
    public int GetN()
    {
        return columns;
    }
    public float GetHexOffset()
    {
        return offset;
    }

    public float GetHexHeight()
    {
        return height_of_hex;
    }

    public List<Hex> GetAllHexesInDirection(Direction direction, Hex center_hex, bool count_unwalkable_fields)
    {
        int range = (columns > rows ? columns : rows) * 2;
        switch (direction)
        {
            case Direction.UP:
                return DirectionHexes(Direction.UP,center_hex,range,count_unwalkable_fields);
            case Direction.DOWN:
                return DirectionHexes(Direction.DOWN, center_hex, range, count_unwalkable_fields);
            case Direction.UPPER_RIGHT:
                return DirectionHexes(Direction.UPPER_RIGHT, center_hex, range, count_unwalkable_fields);
            case Direction.UPPER_LEFT:
                return DirectionHexes(Direction.UPPER_LEFT, center_hex, range, count_unwalkable_fields);
            case Direction.LOWER_RIGHT:
                return DirectionHexes(Direction.LOWER_RIGHT, center_hex, range, count_unwalkable_fields);
            case Direction.LOWER_LEFT:
                return DirectionHexes(Direction.LOWER_LEFT, center_hex, range, count_unwalkable_fields);
            default:
                break;
        }
        return null;
    }
    public List<Hex> GetHexesInDirection(Direction direction, Hex center_hex, int range, bool count_unwalkable_fields)
    {
        switch (direction)
        {
            case Direction.UP:
                return DirectionHexes(Direction.UP, center_hex, range, count_unwalkable_fields);
            case Direction.DOWN:
                return DirectionHexes(Direction.DOWN, center_hex, range, count_unwalkable_fields);
            case Direction.UPPER_RIGHT:
                return DirectionHexes(Direction.UPPER_RIGHT, center_hex, range, count_unwalkable_fields);
            case Direction.UPPER_LEFT:
                return DirectionHexes(Direction.UPPER_LEFT, center_hex, range, count_unwalkable_fields);
            case Direction.LOWER_RIGHT:
                return DirectionHexes(Direction.LOWER_RIGHT, center_hex, range, count_unwalkable_fields);
            case Direction.LOWER_LEFT:
                return DirectionHexes(Direction.LOWER_LEFT, center_hex, range, count_unwalkable_fields);
            default:
                break;
        }
        return null;
    }
    public List<Hex> GetEnemyHexesInDirection(Direction direction, Hex center_hex, ClassType unit_class_type, int range)
    {
        switch (direction)
        {
            case Direction.UP:
                return DirectionEnemyHexes(Direction.UP, center_hex, range, unit_class_type);
            case Direction.DOWN:
                return DirectionEnemyHexes(Direction.DOWN, center_hex, range, unit_class_type);
            case Direction.UPPER_RIGHT:
                return DirectionEnemyHexes(Direction.UPPER_RIGHT, center_hex, range, unit_class_type);
            case Direction.UPPER_LEFT:
                return DirectionEnemyHexes(Direction.UPPER_LEFT, center_hex, range, unit_class_type);
            case Direction.LOWER_RIGHT:
                return DirectionEnemyHexes(Direction.LOWER_RIGHT, center_hex, range, unit_class_type);
            case Direction.LOWER_LEFT:
                return DirectionEnemyHexes(Direction.LOWER_LEFT, center_hex, range, unit_class_type);
            default:
                break;
        }
        return null;
    }
    private List<Hex> DirectionHexes(Direction direction, Hex center_hex, int range, bool count_unwalkable_fields)
    {
        List<Hex> direction_hexes = new List<Hex>();
        Vector2Int direction_coordinates = DirectionToCoordinates(direction);

        for (int i = 1; i < range + 1; i++)
        {
            Hex hex = GetHex(i * direction_coordinates.x + center_hex.coordinates.x, i * direction_coordinates.y + center_hex.coordinates.y);
            if (hex != null)
            {
                if (count_unwalkable_fields)
                    direction_hexes.Add(hex);
                else
                {
                    if (hex.IsWalkable())
                        direction_hexes.Add(hex);
                    else
                        break;
                }
            }
        }
        return direction_hexes;
    }

    private List<Hex> DirectionEnemyHexes(Direction direction, Hex center_hex, int range, ClassType unit_class_type)
    {
        List<Hex> direction_hexes = new List<Hex>();
        Vector2Int direction_coordinates = DirectionToCoordinates(direction);

        for (int i = 1; i < range + 1; i++)
        {
            Hex hex = GetHex(i * direction_coordinates.x + center_hex.coordinates.x, i * direction_coordinates.y + center_hex.coordinates.y);
            if (hex != null && hex.GetUnit() != null && hex.GetUnit().class_type != unit_class_type)
                direction_hexes.Add(hex);
        }
        return direction_hexes;
    }
    public Vector2Int TransformCoordinatesToUnitCoordinates(Vector2Int coordinates)
    {
        int x = coordinates.x != 0 ? Math.Sign(coordinates.x) : 0;
        int y = coordinates.y != 0 ? Math.Sign(coordinates.y) : 0;

        return new Vector2Int(x, y);
    }
    public Vector2Int DirectionToCoordinates(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return new Vector2Int(0, 1);
            case Direction.DOWN:
                return new Vector2Int(0, -1);
            case Direction.UPPER_RIGHT:
                return new Vector2Int(1, -1);
            case Direction.UPPER_LEFT:
                return new Vector2Int(-1, 0);
            case Direction.LOWER_RIGHT:
                return new Vector2Int(1, 0);
            case Direction.LOWER_LEFT:
                return new Vector2Int(-1, 1);
            default:
                return new Vector2Int();
        }
    }

    public Direction CoordinatesToDirection(Vector2Int coordinates)
    {
        if (coordinates == new Vector2Int(0, 1))
        {
            return Direction.UP;
        }
        else if (coordinates == new Vector2Int(0, -1))
        {
            return Direction.DOWN;
        }
        else if (coordinates == new Vector2Int(1, -1))
        {
            return Direction.UPPER_RIGHT;
        }
        else if (coordinates == new Vector2Int(-1, 0))
        {
            return Direction.UPPER_LEFT;
        }
        else if (coordinates == new Vector2Int(1, 0))
        {
            return Direction.LOWER_RIGHT;
        }
        else if (coordinates == new Vector2Int(-1, 1))
        {
            return Direction.LOWER_LEFT;
        }
        else
        {
            throw new ArgumentException("Invalid coordinates");
        }
    }
}

public enum Direction
{
    UP,
    DOWN,
    UPPER_RIGHT,
    UPPER_LEFT,
    LOWER_RIGHT,
    LOWER_LEFT    
}



