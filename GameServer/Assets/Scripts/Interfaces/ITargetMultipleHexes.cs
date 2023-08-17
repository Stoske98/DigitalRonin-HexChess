using Newtonsoft.Json;
using System.Collections.Generic;

public interface ITargetMultipleHexes
{
    [JsonIgnore] public List<Hex> targetable_hexes { get; set; }
    public void SetAbility(List<Hex> _targetable_hexes)
    {
        targetable_hexes = _targetable_hexes;
    }
}


