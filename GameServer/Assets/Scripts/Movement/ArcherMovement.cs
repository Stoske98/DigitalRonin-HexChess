using System.Collections.Generic;

public class ArcherMovement : MovementBehaviour
{
    public ArcherMovement() : base() { }
    public ArcherMovement(Unit _unit) : base(_unit)
    {
        range = 2;
    }
    public ArcherMovement(Unit _unit, int _range) : base(_unit)
    {
        range = _range;
    }
    public override List<Hex> GetAvailableMoves(Hex _unit_hex)
    {
        List<Hex> hexes = new List<Hex>();
        Map map = NetworkManager.Instance.games[unit.match_id].map;
        for (int i = 1; i <= range; i++)
        {
            foreach (var diagonal in map.diagonals_neighbors_vectors)
            {
                Hex hex = map.GetHex(_unit_hex.coordinates.x + diagonal.x * i, _unit_hex.coordinates.y + diagonal.y * i);
                if (hex != null && hex.IsWalkable())
                    hexes.Add(hex);
            }
        }
        return hexes;
    }

    public override void SetPath(Hex _unit_hex, Hex _desired_hex)
    {
        base.SetPath(_unit_hex, _desired_hex);

        path.Clear();
        path.Add(_unit_hex);
        path.Add(_desired_hex);

        unit.events.OnStartMovement_Local?.Invoke(_unit_hex, _desired_hex);
    }
}




