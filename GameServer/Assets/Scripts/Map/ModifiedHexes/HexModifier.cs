using Newtonsoft.Json;
using UnityEngine;

public abstract class HexModifier : IObject
{
    public string id { get; set; }
    public string game_object_path { get; set; }
    public bool should_be_removed { get; set; }
    public Visibility visibility { get; set; }
    public ClassType class_type { get; set; }
    [JsonConverter(typeof(CustomConverters.GameObjectConverter))] public GameObject game_object { get; set ; }
    public abstract void Trigger(Unit _unit, Hex _hex);
    public abstract void Update();
}

