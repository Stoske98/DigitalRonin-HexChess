using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeRoyaleGame : Game
{
    public int start_countdowm = 30;
    public bool is_activated { set; get; }
    [JsonRequired] private int counter = 0;
    [JsonRequired] private int remove_on_turn = 10;
    [JsonRequired] private int n;

    [JsonConstructor] public ChallengeRoyaleGame() : base() { }
    public ChallengeRoyaleGame(Map _map, int _start_countdowm, int _remove_on_turn) : base(_map)
    {
        start_countdowm = _start_countdowm;
        remove_on_turn = _remove_on_turn;
        n = map.GetN();
       /* Grall grall = new Grall();
        GetHex(0, 0).PlaceObject(grall);*/
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
        Hex center_hex = GetHex(0, 0);
        List<Hex> inner_hexes = HexesInRange(center_hex, n - 1);
        n--;

        List<Hex> outer_hexes = new List<Hex>();
        foreach (var hex in GetMapHexes())
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
            GetMapHexes().Remove(hex);
    }

    public override void EndTurn()
    {
        move++;
        class_on_turn = class_on_turn == ClassType.LIGHT ? ClassType.DARK : ClassType.LIGHT;

        foreach (Unit unit in units)
        {
            if(unit.class_type == class_on_turn)
            {
                unit.UpdateAbilitiesCooldown();
                unit.UpdateCCsCooldown();
            }
        }

        game_events.OnEndTurn?.Invoke();
    }
}


