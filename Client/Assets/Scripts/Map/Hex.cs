using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Hex
{ 
    public Vector2Int coordinates { set; get; }
    public int S { set; get; }
    [JsonRequired] private bool is_walkable { set; get; }
    [JsonConverter(typeof(CustomConverters.GameObjectConverter))] public GameObject game_object { set; get; }
    [JsonRequired] [JsonConverter(typeof(CustomConverters.ObjectListConverter))] public List<IObject> objects { get; set; }
    [JsonIgnore]  public HexPathData path_data{ set; get; }

    [JsonIgnore] public MeshRenderer hex_mesh;
    [JsonIgnore] private Color base_color;

    [JsonConstructor]
    public Hex() 
    {
        path_data = new HexPathData();
        objects = new List<IObject>();
        //is_walkable = true;
    }
    public Hex(Vector2Int _coordinates)
    {
        coordinates = _coordinates;
        S = -coordinates.x - coordinates.y;
        path_data = new HexPathData();
        objects = new List<IObject>();
        is_walkable = true;
    }

    public void CreateHexGameObject(Vector3 center, float height)
    {
        game_object = new GameObject();
        game_object.transform.position = center;
        game_object.name = "Tile[" + coordinates.x + "][" + coordinates.y + "]";

        Mesh mesh = game_object.AddComponent<MeshFilter>().mesh;
        hex_mesh = game_object.AddComponent<MeshRenderer>();

        mesh.Clear();

        float angle = 0;
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * height;
            angle += 60;

        }
        mesh.vertices = vertices;

        mesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };

        mesh.RecalculateNormals();

        game_object.transform.localEulerAngles = new Vector3(-90, 0, 0);

        SetMaterial(GameManager.Instance.map_controller.field_material);
        game_object.transform.SetParent(GameManager.Instance.map_controller.fields_container);
    }
    public List<Hex> GetNeighbors(Map map)
    {
        List<Hex> neighbors = new List<Hex>();
        foreach (Vector2Int vector2 in map.neighbors_vectors)
        {
            Hex neighbor = map.GetHex(coordinates.x + vector2.x, coordinates.y + vector2.y);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    public void SetMaterial(Material _material)
    {
        base_color = _material.color;
        hex_mesh.material.color = _material.color;
    }

    public void SetColor(Color _color)
    {
        hex_mesh.material.color = _color;
    }

    public void ResetColor()
    {
        hex_mesh.material.color = base_color;
    }

    public Unit GetUnit()
    {
        foreach (IObject obj in objects)
        {
            if (obj is Unit unit)
                return unit;
        }
        return null;
    }

    public void PlaceObject(IObject _obj)
    {
        objects.Add(_obj);

        if (is_walkable)
            foreach (var obj in objects)
                if (obj is Unit)
                    is_walkable = false;
    }

    public void RemoveObject(IObject _obj)
    {
        objects.RemoveAll(obj => obj == _obj);
        
        if (!is_walkable)
            foreach (var obj in objects)
                if (obj is Unit)
                    return;

        is_walkable = true;
    }
    public void TriggerModifier(Unit _unit)
    {
        foreach (IObject obj in objects.ToArray())
        {
            if (obj is HexModifier modifier)
            {
                modifier.Trigger(_unit, this);
                if (modifier.should_be_removed)
                    GameManager.Instance.game.object_manager.RemoveObject(modifier);
            }
        }

        objects.RemoveAll(obj => obj.GetType().IsSubclassOf(typeof(HexModifier)) && ((HexModifier)obj).should_be_removed == true);
    }
    public bool IsWalkable()
    {
        return is_walkable;
    }

}

public class HexPathData
{
    public int weight = 1;
    public int cost;
    public Hex prev_hex;
}

