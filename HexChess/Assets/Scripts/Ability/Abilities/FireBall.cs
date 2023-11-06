using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : TargetableAbility, ITargetableSingleHex
{
    string path = "Prefabs/Wizard/Light/Ability/FireBall";
    GameObject vfx_prefab;
    List<Unit> enemies;
    Vector3 direction;
    float speed = 20;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public FireBall() : base() 
    { 
        enemies = new List<Unit>();
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public FireBall(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) 
    {
        enemies = new List<Unit>();
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public override void Execute()
    {
        GameObject game_object = Object.Instantiate(vfx_prefab, unit.game_object.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        float distance = Vector3.Distance(game_object.transform.position, direction);
        game_object.LeanMove(direction + Vector3.up * 1.5f, distance / speed);
        //Object.Destroy(game_object, distance / speed);

        foreach (Unit enemy_unit in enemies)
            enemy_unit.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

        enemies.Clear();        
        Exit();
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Game game = GameManager.Instance.game;

        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.UP, _unit_hex));
        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.DOWN, _unit_hex));
        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.LOWER_LEFT, _unit_hex));
        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.UPPER_LEFT, _unit_hex));
        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _unit_hex));
        _ability_moves.AddRange(game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _unit_hex));

        return _ability_moves;
    }
    public void SetAbility(Hex _target_hex)
    {
        Game game = GameManager.Instance.game;
        Hex _cast_unit_hex = game.map.GetHex(unit);
        if(_cast_unit_hex != null)
        {
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.UP, _cast_unit_hex), ref enemies);
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.DOWN, _cast_unit_hex), ref enemies);
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.LOWER_LEFT, _cast_unit_hex), ref enemies);
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.UPPER_LEFT, _cast_unit_hex), ref enemies);
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.LOWER_RIGHT, _cast_unit_hex), ref enemies);
            CheckIsEnemiesOnDirection(_target_hex, game.GetAllHexesInDirection(Direction.UPPER_RIGHT, _cast_unit_hex), ref enemies);

        }
    }


    private void CheckIsEnemiesOnDirection(Hex _target_hex, List<Hex> _hexes_in_direction, ref List<Unit> _enemy_units)
    {
        if (_hexes_in_direction.Contains(_target_hex))
        {
            direction = _hexes_in_direction[^1].game_object.transform.position;
            foreach (Hex hex in _hexes_in_direction)
                if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                    _enemy_units.Add(hex.GetUnit());            
        }
    }
}