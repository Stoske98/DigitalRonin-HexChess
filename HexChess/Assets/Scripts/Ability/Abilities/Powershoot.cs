using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Powershoot : TargetableAbility, ITargetableSingleHex
{
    string path = "Prefabs/Archer/Light/Ability/powershoot";
    GameObject vfx_prefab;
    float speed = 20;
    Unit enemy = null;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Powershoot() : base()
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Powershoot(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public override void Execute()
    {
        GameObject game_object = Object.Instantiate(vfx_prefab, unit.game_object.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        game_object.transform.LookAt(enemy.game_object.transform.position);
        float distance = Vector3.Distance(game_object.transform.position, enemy.game_object.transform.position);
        game_object.LeanMove(enemy.game_object.transform.position + Vector3.up * 1.5f, distance / speed);
        Object.Destroy(game_object, distance / speed);

        if (enemy != null)
            enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

        enemy = null;
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = GameManager.Instance.game;

        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UP, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.DOWN, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UPPER_LEFT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _unit_hex)));
        _ability_moves.AddRange(AvailableMovesInDirection(_unit_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _unit_hex)));

        return _ability_moves;
    }
    public void SetAbility(Hex _target_hex)
    {
        enemy = TryToGetEnemyUnit(_target_hex);
    }

    private List<Hex> AvailableMovesInDirection(Hex center, List<Hex> hexes)
    {
        int count = 0;
        for (int i = 0; i < hexes.Count; i++)
        {
            if(!hexes[i].IsWalkable() && hexes[i].GetUnit().class_type != center.GetUnit().class_type)
            {
                count = i;
                break;
            }

            if (i == hexes.Count - 1)
            {
                hexes.Clear();
                return hexes;
            }
        }
        for (int i = hexes.Count - 1; i > count; i--)
            hexes.RemoveAt(i);

        return hexes;
    }

    private Unit TryToGetEnemyUnit(Hex target_hex)
    {
        Game game = GameManager.Instance.game;
        Hex _cast_unit_hex = game.map.GetHex(unit);
        if(_cast_unit_hex != null)
        {
            Unit enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.UP, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.DOWN, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
            enemy = CheckIsEnemyOnDirection(target_hex, game.GetAllHexesInDirection(Direction.UPPER_LEFT, _cast_unit_hex));
            if (enemy != null)
                return enemy;
        }       

        return null;
    }

    private Unit CheckIsEnemyOnDirection(Hex target_hex, List<Hex> hexes)
    {
        if (hexes.Contains(target_hex))
            foreach (Hex hex in hexes)
                if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                    return hex.GetUnit();

        return null;
    }
}
