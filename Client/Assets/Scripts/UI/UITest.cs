using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    [Header("Unit Data")]
    public Text unit_name;
    public Text unit_class;
    public Text unit_coordinates;
    public Text unit_max_hp;
    public Text unit_hp;
    public Text unit_damage;


    [Header("End Turn")]
    public Text move;
    public Text class_on_turn;


   /* private void Start()
    {
        Invoke("Subscribe", 2.0f);
    }

    public void Subscribe()
    {
        GameManager.Instance.OnSelectUnit += DisplayUnit;
        GameManager.Instance.game.game_events.OnEndTurnDisplay += DisplayOnEndTurn;
        GameManager.Instance.game.game_events.OnEndMovement_Global += DisplayUnit;
        DisplayOnEndTurn(GameManager.Instance.game);
    }
    public void DisplayUnit(Hex hex)
    {
        if(hex.GetUnit() == GameManager.Instance.selected_unit)
        {
            unit_name.text = "Name: " + hex.GetUnit()?.GetType().ToString();
            unit_class.text = "Class: " + hex.GetUnit()?.class_type.ToString();
            unit_coordinates.text = "Coordinates: [" + hex.coordinates.x + "][" + hex.coordinates.y + "]";
            unit_max_hp.text = "Max HP: " + hex.GetUnit()?.stats.max_health;
            unit_hp.text = "HP: " + hex.GetUnit()?.stats.current_health;
            unit_damage.text = "Damage: " + hex.GetUnit()?.stats.damage;

        }
    }

    public void DisplayOnEndTurn(Game game)
    {
        move.text = "Move: " + game.move;
        class_on_turn.text = "Class On Turn: " + game.class_on_turn.ToString();
    }*/
}
