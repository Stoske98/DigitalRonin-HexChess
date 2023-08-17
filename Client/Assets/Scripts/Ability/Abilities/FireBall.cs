using Newtonsoft.Json;
using System.Collections.Generic;

public class FireBall : TargetableAbility, ITargetableSingleHex
{
    List<Unit> enemies;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public FireBall() : base() { enemies = new List<Unit>(); }
    public FireBall(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data) { enemies = new List<Unit>(); }
    public override void Execute()
    {

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
            foreach (Hex hex in _hexes_in_direction)
                if (!hex.IsWalkable() && hex.GetUnit().class_type != unit.class_type)
                    _enemy_units.Add(hex.GetUnit());
    }
}