using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Vampirism : TargetableAbility, ITargetableSingleHex
{
    string path = "Prefabs/Wizard/Dark/Ability/Vampirism";
    GameObject vfx_prefab;
    GameObject vfx_game_object;
    List<Vector3> vfx_path;
    float speed = 20;
    int current_index = 0;
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public Vampirism() : base()
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }
    public Vampirism(Unit _unit, AbilityData _ability_data, string _sprite_path) : base(_unit, _ability_data, _sprite_path)
    {
        vfx_prefab = Resources.Load<GameObject>(path);
    }

    public override void Execute()
    {
        Map map = GameManager.Instance.game.map;
        List<Hex> longest_enemy_path = PathFinding.PathFinder.BFS_LongestEnemyUnitPath(targetable_hex, unit, map, 5);

        vfx_game_object = GameObject.Instantiate(vfx_prefab, unit.game_object.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        vfx_path = new List<Vector3>() { unit.game_object.transform.position + Vector3.up * 1.5f };
        foreach (var hex in longest_enemy_path)
            vfx_path.Add(hex.game_object.transform.position + Vector3.up * 1.5f);
        current_index = 0;
        MoveToNextPosition();

        for (int i = 0; i < 5; i++)
        {
            if(i < longest_enemy_path.Count)
            {
                Unit enemy = longest_enemy_path[i].GetUnit();
                enemy.ReceiveDamage(new MagicDamage(unit, ability_data.amount));

                if (unit.stats.current_health + ability_data.amount > unit.stats.max_health)
                    unit.stats.current_health = unit.stats.max_health;
                else
                    unit.stats.current_health += ability_data.amount;
            }
        }
        GameManager.Instance.game.game_events.OnChangeUnitData_Global?.Invoke(unit);
        Exit();
    }
    private void MoveToNextPosition()
    {
        if(current_index < vfx_path.Count)
        {
            Vector3 nextPosition = vfx_path[current_index];
            vfx_game_object.transform.LookAt(nextPosition);
            // Tween the object to the next position
            LeanTween.move(vfx_game_object, nextPosition, Vector3.Distance(vfx_game_object.transform.position, nextPosition) / speed)
                .setEase(LeanTweenType.linear)
                .setOnComplete(OnMoveComplete);

            current_index++;
        }else { GameObject.Destroy(vfx_game_object, 0.1f); }
    }
    private void OnMoveComplete()
    {
        MoveToNextPosition();
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = new List<Hex>();

        foreach (Hex hex in GameManager.Instance.game.map.HexesInRange(_unit_hex, ability_data.range))
        {
            Unit enemy = hex.GetUnit();
            if (enemy != null && unit.class_type != enemy.class_type)
                _available_moves.Add(hex);
        }

        return _available_moves;
    }
}
