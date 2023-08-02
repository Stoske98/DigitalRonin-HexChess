using Newtonsoft.Json;
using UnityEngine;

public class ChallengeRoyaleMap : Map
{
    [JsonConstructor] public ChallengeRoyaleMap() : base() { }
    public ChallengeRoyaleMap(int _columns, int _rows, float _offset, float _height_of_hex) : base(_columns, _rows, _offset, _height_of_hex) { CreateMap(); }
    public override void CreateMap()
    {
        Vector3 pos = Vector3.zero;
        for (int c = -columns; c <= columns; c++)
        {
            int r1 = Mathf.Max(-columns, -c - columns);
            int r2 = Mathf.Min(columns, -c + columns);

            for (int r = r1; r <= r2; r++)
            {
                pos.x = height_of_hex * 3.0f / 2.0f * c * offset;
                pos.z = height_of_hex * Mathf.Sqrt(3.0f) * (r + c / 2.0f) * offset;

                Vector2Int coordinates = new Vector2Int(c, r);

                Hex hex = new Hex(coordinates);
                hex.CreateHexGameObject(pos, height_of_hex);

                hexes.Add(hex);
            }
        }

        foreach (Hex hex in hexes)
            hex.SetNeighbors(this);
    }

    public override void SpawnUnits(Game _game)
    {
        //LIGHT
        Spawner.SpawnUnit(_game, UnitType.KING, ClassType.LIGHT, _game.GetHex(0, -4));
        Spawner.SpawnUnit(_game, UnitType.QUEEN, ClassType.LIGHT, _game.GetHex(0, -3));

        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.LIGHT, _game.GetHex(0, -2));
        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.LIGHT, _game.GetHex(-2, -1));
        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.LIGHT, _game.GetHex(2, -3));

        Spawner.SpawnUnit(_game, UnitType.TANK, ClassType.LIGHT, _game.GetHex(-1, -3));
        Spawner.SpawnUnit(_game, UnitType.TANK, ClassType.LIGHT, _game.GetHex(1, -4));

        Spawner.SpawnUnit(_game, UnitType.ARCHER, ClassType.LIGHT, _game.GetHex(-3, -1));
        Spawner.SpawnUnit(_game, UnitType.ARCHER, ClassType.LIGHT, _game.GetHex(3, -4));

        Spawner.SpawnUnit(_game, UnitType.KNIGHT, ClassType.LIGHT, _game.GetHex(1, -3));
        Spawner.SpawnUnit(_game, UnitType.KNIGHT, ClassType.LIGHT, _game.GetHex(-1, -2));

        Spawner.SpawnUnit(_game, UnitType.JESTER, ClassType.LIGHT, _game.GetHex(-2, -2));
        Spawner.SpawnUnit(_game, UnitType.WIZARD, ClassType.LIGHT, _game.GetHex(2, -4));

        //DARK
        Spawner.SpawnUnit(_game, UnitType.KING, ClassType.DARK, _game.GetHex(0, 4));
        Spawner.SpawnUnit(_game, UnitType.QUEEN, ClassType.DARK, _game.GetHex(0, 3));

        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.DARK, _game.GetHex(0, 2));
        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.DARK, _game.GetHex(2, 1));
        Spawner.SpawnUnit(_game, UnitType.SWORDSMAN, ClassType.DARK, _game.GetHex(-2, 3));

        Spawner.SpawnUnit(_game, UnitType.TANK, ClassType.DARK, _game.GetHex(1, 3));
        Spawner.SpawnUnit(_game, UnitType.TANK, ClassType.DARK, _game.GetHex(-1, 4));

        Spawner.SpawnUnit(_game, UnitType.ARCHER, ClassType.DARK, _game.GetHex(3, 1));
        Spawner.SpawnUnit(_game, UnitType.ARCHER, ClassType.DARK, _game.GetHex(-3, 4));

        Spawner.SpawnUnit(_game, UnitType.KNIGHT, ClassType.DARK, _game.GetHex(-1, 3));
        Spawner.SpawnUnit(_game, UnitType.KNIGHT, ClassType.DARK, _game.GetHex(1, 2));

        Spawner.SpawnUnit(_game, UnitType.JESTER, ClassType.DARK, _game.GetHex(2, 2));
        Spawner.SpawnUnit(_game, UnitType.WIZARD, ClassType.DARK, _game.GetHex(-2, 4));
    }
}

