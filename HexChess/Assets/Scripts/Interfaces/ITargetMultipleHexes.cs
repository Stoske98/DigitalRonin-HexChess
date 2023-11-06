using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetMultipleHexes
{
    public int max_hexes { get; set; }
    public bool has_condition { get; set; } 
    [JsonIgnore] public List<Hex> targetable_hexes { get; set; }
    [JsonIgnore] public Dictionary<Hex, GameObject> placement { get; set; }
    [JsonIgnore] public GameObject vfx_prefab { get; set; }
    public void SetAbility(List<Hex> _targetable_hexes)
    {
        targetable_hexes = _targetable_hexes;
    }

    public void Place(Hex hex)
    {
        GameObject game_object = Object.Instantiate(vfx_prefab, hex.game_object.transform.position, Quaternion.identity);
        placement.Add(hex, game_object);
    }

    public void Remove(Hex hex)
    {
        Object.Destroy(placement[hex]);
        placement.Remove(hex);
    }

    public void RemoveAll()
    {
        foreach (GameObject go in placement.Values)
            Object.Destroy(go);

        placement.Clear();
    }
}


