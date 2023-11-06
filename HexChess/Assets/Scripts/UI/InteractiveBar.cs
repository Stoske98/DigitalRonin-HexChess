using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBar : MonoBehaviour
{
    public RectTransform hero_interactive_bar;
    public List<AbilityController> abilities_controller;

    private Vector3 left_up = new Vector3(-60, 115 ,0);
    private Vector3 up = new Vector3(0, 150, 0);
    private Vector3 right_up = new Vector3(60, 115, 0);
    private Vector3 down = new Vector3(0, 80, 0);
    private Vector3 center = new Vector3(0, 80, 0);
    public void SetBar(Unit unit)
    {
        Vector3 screenPos = GameManager.Instance.map_controller.cm.WorldToScreenPoint(unit.game_object.transform.position);
        hero_interactive_bar.position = screenPos;

        unit.health_bar_controller.HideHealthBar();

        List<AbilityBehaviour> ability_behaviours = new List<AbilityBehaviour>();

        foreach (var behaviour in unit.behaviours)
            if (behaviour is AbilityBehaviour ability_behaviour)
                ability_behaviours.Add(ability_behaviour);

        if(ability_behaviours.Count == 1)
        {
            for (int i = 0; i < abilities_controller.Count; i++)
                if (i > ability_behaviours.Count - 1)
                    abilities_controller[i].gameObject.SetActive(false);

            abilities_controller[0].SetAbility(ability_behaviours[0]);
            abilities_controller[0].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = center;
            abilities_controller[0].gameObject.SetActive(true);
        }
        else if (ability_behaviours.Count == 2)
        {
            for (int i = 0; i < abilities_controller.Count; i++)
                if (i > ability_behaviours.Count - 1)
                    abilities_controller[i].gameObject.SetActive(false);

            abilities_controller[0].SetAbility(ability_behaviours[0]);
            abilities_controller[0].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = down;
            abilities_controller[0].gameObject.SetActive(true);

            abilities_controller[1].SetAbility(ability_behaviours[1]);
            abilities_controller[1].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = up;
            abilities_controller[1].gameObject.SetActive(true);
        }
        else if(ability_behaviours.Count == 3)
        {
            for (int i = 0; i < abilities_controller.Count; i++)
                if (i > ability_behaviours.Count - 1)
                    abilities_controller[i].gameObject.SetActive(false);

            abilities_controller[0].SetAbility(ability_behaviours[0]);
            abilities_controller[0].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = down;
            abilities_controller[0].gameObject.SetActive(true);

            abilities_controller[1].SetAbility(ability_behaviours[1]);
            abilities_controller[1].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = new Vector3(-32.5f, 135, 0);
            abilities_controller[1].gameObject.SetActive(true);

            abilities_controller[2].SetAbility(ability_behaviours[2]);
            abilities_controller[2].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = new Vector3(32.5f, 135, 0);
            abilities_controller[2].gameObject.SetActive(true);
        }
        else if (ability_behaviours.Count == 4)
        {
            for (int i = 0; i < abilities_controller.Count; i++)
                if (i > ability_behaviours.Count - 1)
                    abilities_controller[i].gameObject.SetActive(false);

            abilities_controller[0].SetAbility(ability_behaviours[0]);
            abilities_controller[0].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = down;
            abilities_controller[0].gameObject.SetActive(true);

            abilities_controller[1].SetAbility(ability_behaviours[1]);
            abilities_controller[1].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = left_up;
            abilities_controller[1].gameObject.SetActive(true);

            abilities_controller[2].SetAbility(ability_behaviours[2]);
            abilities_controller[2].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = up;
            abilities_controller[2].gameObject.SetActive(true);

            abilities_controller[3].SetAbility(ability_behaviours[3]);
            abilities_controller[3].gameObject.gameObject.GetComponent<RectTransform>().transform.localPosition = right_up;
            abilities_controller[3].gameObject.SetActive(true);

        }
       
        hero_interactive_bar.gameObject.SetActive(true);
    }
    public void HideBar()
    {
        hero_interactive_bar.gameObject.SetActive(false);
    }
    public void OnHoverAbility(int position)
    {    
        GameManager game_manager = GameManager.Instance;

        if (game_manager.player_input_handler.GetSelectedUnit() == null)
            return;

        if (/*game_manager.player_input_handler.GetSelectedUnit().class_type == NetworkManager.Instance.player.data.class_type &&*/ abilities_controller[position].ability is Ability ability)
        {
            game_manager.map_controller.ResetFields();
            game_manager.map_controller.MarkAbilityMoves(ability, game_manager.player_input_handler.GetSelectedHex());
        }
    }
    public void OnUnHoverAbility()
    {
        GameManager game_manager = GameManager.Instance;
        game_manager.map_controller.ResetFields();
        game_manager.map_controller.MarkSelectedHex(game_manager.player_input_handler.GetSelectedHex());
    }
    public void OnPressAbility(int position)
    {
        GameManager game_manager = GameManager.Instance;

        if (game_manager.player_input_handler.GetSelectedUnit() == null)
            return;
        if (abilities_controller[position].ability is Ability ability)
        {
            KeyCode key_code = KeyCode.None;
            if(position == 0)
                key_code = KeyCode.S;
            else if(position == 1)
                key_code = KeyCode.Q;
            else if (position == 2)
                key_code = KeyCode.W;
            else if (position == 3)
                key_code = KeyCode.E;

            game_manager.player_input_handler.SwitchActionMap("SelectedUnitHandler");
            game_manager.player_input_handler.OnAbilityPress(key_code);
            hero_interactive_bar.gameObject.SetActive(false);
        }
    }
}
