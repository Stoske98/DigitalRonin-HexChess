using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Blink : TargetableAbility, ITargetableSingleHex, IUpgradable
{
    string path = "Prefabs/Swordsman/Dark/Ability/Blink";
    GameObject vfx_prefab;
    [JsonIgnore] public Hex targetable_hex { get; set; }

    public Blink() : base()
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Blink(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path) 
    { 
        vfx_prefab = Resources.Load<GameObject>(path);
    }

    public override void Execute()
    {
        Hex _unit_hex = GameManager.Instance.game.map.GetHex(unit);

        BlinkMovement blink = new BlinkMovement(unit, 2);
        blink.SetPath(_unit_hex, targetable_hex);
        unit.AddBehaviourToWork(blink);

        if (unit.level == 3)
            blink.OnEndBehaviour += OnEndOfMovementJuggernaut;

        targetable_hex = null;
        Exit();
    }

    private void OnEndOfMovementJuggernaut(Behaviour behaviour)
    {
        behaviour.OnEndBehaviour -= OnEndOfMovementJuggernaut;
        if (!unit.IsDead())
        {
            Map map = GameManager.Instance.game.map;
            Hex unit_hex = map.GetHex(unit);
            unit.animator.Play("Blink_Jugg_Dark_Swordsman");
            if(unit_hex != null)
            {
                Object.Instantiate(vfx_prefab, unit.game_object.transform.position + Vector3.up, unit.game_object.transform.rotation);
                foreach (Hex _hex in GameManager.Instance.game.map.HexesInRange(unit_hex, ability_data.range))
                {
                    Unit enemy = _hex.GetUnit();
                    if (enemy != null && enemy.class_type != unit.class_type)
                        enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

                }
            }
        }
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _ability_moves = new List<Hex>();

        Map map = GameManager.Instance.game.map;
        foreach (var diagonal_coordinate in map.diagonals_neighbors_vectors)
        {
            Hex _desired_hex = map.GetHex(_unit_hex.coordinates.x + diagonal_coordinate.x, _unit_hex.coordinates.y + diagonal_coordinate.y);
            if (_desired_hex != null && _desired_hex.IsWalkable())
                _ability_moves.Add(_desired_hex);
        }

        unit.events.OnGetAbilityMoves_Local?.Invoke(_unit_hex, _ability_moves);
        return _ability_moves;
    }

    public void Upgrade()
    {
        ability_data.amount = 1;
        ability_data.max_cooldown = 3;
    }
}