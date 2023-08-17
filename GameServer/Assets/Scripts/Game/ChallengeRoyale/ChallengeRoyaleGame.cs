using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
public class ChallengeRoyaleGame : Game
{
    public int start_countdowm = 30;
    public bool is_activated { set; get; }
    public ShardController shard_controller { get; set; }

    [JsonRequired] private int counter = 0;
    [JsonRequired] private int remove_on_turn = 10;
    [JsonRequired] private int n;
    public List<Vector2Int> exp_flag_coordinates { set; get; }
    [JsonConstructor] public ChallengeRoyaleGame() : base() { }
    public ChallengeRoyaleGame(int _match_id, Map _map, int _start_countdowm, int _remove_on_turn) : base(_match_id, _map)
    {
        start_countdowm = _start_countdowm;
        remove_on_turn = _remove_on_turn;
        n = map.GetN();

        shard_controller = new ShardController();
        shard_controller.InitControllers();

        exp_flag_coordinates = new List<Vector2Int>() { new Vector2Int(0, 2), new Vector2Int(0, -2), new Vector2Int(-2,0), new Vector2Int(2, -2), new Vector2Int(2, 0), new Vector2Int(-2, 2), };
    }
    public override void Init()
    {
        map.SpawnUnits(this);
        object_manager.ProcessPendingActions();
    }
    public void Activate()
    {
        if (!is_activated)
        {
            is_activated = true;
            game_events.OnEndTurn += Countdown;
        }
    }

    public void Countdown()
    {
        counter++;

        if (ShouldRemoveOuterFields() && n > 1)
            RemoveOuterFields();

        if (n == 1)
            game_events.OnEndTurn -= Countdown;

    }

    private bool ShouldRemoveOuterFields()
    {
        if (counter == remove_on_turn)
        {
            counter = 0;
            return true;
        }

        return false;
    }

    private void RemoveOuterFields()
    {
        Hex center_hex = map.GetHex(0, 0);
        List<Hex> inner_hexes = map.HexesInRange(center_hex, n - 1);
        n--;

        List<Hex> outer_hexes = new List<Hex>();
        foreach (var hex in map.hexes)
        {
            if (!inner_hexes.Contains(hex))
            {
                Unit unit = hex.GetUnit();
                if (unit != null)
                {
                    unit.stats.current_health = 0;
                    //play death and remove figures
                }

                hex.game_object.SetActive(false);
                outer_hexes.Add(hex);
            }
        }

        foreach (Hex hex in outer_hexes)
            map.hexes.Remove(hex);
    }

    public override void EndTurn()
    {
        move++;
        class_on_turn = class_on_turn == ClassType.Light ? ClassType.Dark : ClassType.Light;

        UpdateFlagExp();

        foreach (var obj in object_manager.objects)
        {
            if(obj is Unit unit && unit.class_type == class_on_turn)
            {
                unit.UpdateAbilitiesCooldown();
                unit.UpdateCCsCooldown();
            }
        }

        game_events.OnEndTurn?.Invoke();
        action_done = false;

        NetEndTurn responess = new NetEndTurn();
        responess.class_on_turn = class_on_turn;
        foreach (Player player in players)
            Sender.SendToClient_Reliable((ushort)player.connection_id, responess);
    }

    private void UpdateFlagExp()
    {
        foreach (Vector2Int coordiante in exp_flag_coordinates)
        {
            Unit unit = map.GetHex(coordiante.x, coordiante.y)?.GetUnit();
            if (unit != null && class_on_turn == unit.class_type) 
            {
                if (class_on_turn == ClassType.Light)
                    shard_controller.light_shards++;
                else if (class_on_turn == ClassType.Dark)
                    shard_controller.dark_shards++;
            }
        }
    }

}


