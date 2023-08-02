using Newtonsoft.Json;
using UnityEngine;

public abstract class HexModifier : IObject
{
    public string id { get; set; }
    public bool should_be_removed { get; set; }
    public GameObject game_object { get; set ; }
    public abstract void Trigger(Unit _unit, Hex _hex);

}

