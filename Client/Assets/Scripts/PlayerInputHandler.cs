using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public GameManager game_manager;

    public PlayerInput player_input_controller;
    private Hex selected_hex { get; set; }
    private Unit selected_unit { get; set; }
    private TargetableAbility targetable_ability { get; set; }
    private KeyCode key_code = KeyCode.None;

    public void SwitchActionMap(string new_action_map)
    {
        Debug.Log("Current map DISABLE: " + player_input_controller.currentActionMap.name);
        player_input_controller.currentActionMap?.Disable();
        player_input_controller.currentActionMap = player_input_controller.actions.FindActionMap(new_action_map);
        player_input_controller.currentActionMap?.Enable();
        Debug.Log("Current map ENABLE: " + player_input_controller.currentActionMap.name);
    }
    public void OnMouseScreenPosition(InputAction.CallbackContext value)
    {
        game_manager.map_controller.mouse_screen_position = value.ReadValue<Vector2>();
    }
    public void OnSelectBasicHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();
            Unit _unit = _hex?.GetUnit();

            //SELECT UNIT
            if (_hex != null && _unit != null)
            {
                game_manager.map_controller.ResetFields();
                SelectUnit(_unit,_hex);

                SwitchActionMap("SelectedUnitHandler");
            }
        }
    }
    public void OnDeselectBasicHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
        }
    }

    public void OnSelectUnitHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();
            Unit _unit = _hex?.GetUnit();

            //SELECT UNIT
            if (_hex != null && _unit != null && _unit != selected_unit)
            {
                game_manager.map_controller.ResetFields();
                SelectUnit(_unit, _hex);
                return;
            }
            //MOVE           
            if (_hex != null && _hex.IsWalkable() && !selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
            {
                MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
                if(movement_behaviour != null && movement_behaviour.GetAvailableMoves(selected_hex).Contains(_hex))
                {
                    game_manager.map_controller.ResetFields();

                    NetMove request = new NetMove()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        desired_col = _hex.coordinates.x,
                        desired_row = _hex.coordinates.y
                    };
                    Sender.SendToServer_Reliable(request);
                }
            }
        }
    }

    public void OnDeselectUnitHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();
            Unit _unit = _hex?.GetUnit();

            if (_unit != null && selected_unit.class_type == game_manager.game.class_on_turn)
            {
                if(!selected_unit.IsWork() && _unit.class_type != selected_unit.class_type)
                {
                    AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>() as AttackBehaviour;
                    if (attack_behaviour != null && attack_behaviour.GetAttackMoves(selected_hex).Contains(_hex))
                    {
                        game_manager.map_controller.ResetFields();

                        NetAttack request = new NetAttack()
                        {
                            attacker_id = selected_unit.id,
                            attacker_col = selected_hex.coordinates.x,
                            attacker_row = selected_hex.coordinates.y,
                            target_id = _unit.id,
                            target_col = _hex.coordinates.x,
                            target_row = _hex.coordinates.y
                        };
                        Sender.SendToServer_Reliable(request);
                    }
                }              

                return;
            }

            DeselectUnit();
        }
    }
    public void OnSpecialAbilityHandler(InputAction.CallbackContext value)
    {
        if(value.started)
            OnAbilityPress(KeyCode.S);
    }
    public void OnAbility1Handler(InputAction.CallbackContext value)
    {
        if (value.started)
            OnAbilityPress(KeyCode.Q);
    }
    public void OnAbility2Handler(InputAction.CallbackContext value)
    {
        if (value.started)
            OnAbilityPress(KeyCode.W);
    }
    public void OnAbility3Handler(InputAction.CallbackContext value)
    {
        if (value.started)
            OnAbilityPress(KeyCode.E);
    }
    public void OnAbility4Handler(InputAction.CallbackContext value)
    {
        if (value.started)
            OnAbilityPress(KeyCode.R);
    }
    public void OnSelectTargetableAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();

            if (_hex != null && targetable_ability.GetAbilityMoves(selected_hex).Contains(_hex))
            {
                NetTargetableAbilility request = new NetTargetableAbilility()
                {
                    unit_id = selected_unit.id,
                    col = selected_hex.coordinates.x,
                    row = selected_hex.coordinates.y,
                    desired_col = _hex.coordinates.x,
                    desired_row = _hex.coordinates.y,
                    key_code = key_code
                };
                Sender.SendToServer_Reliable(request);

                DeselectTargetableAbility();
            }
        }
    }
    public void OnDeselectTargetableAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            DeselectTargetableAbility();

            if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
                game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, selected_hex);
        }
    }
    public void SelectUnit(Unit _unit, Hex _hex)
    {
        selected_hex = _hex;
        selected_unit = _unit;

        if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
            game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, _hex);
    }

    public void DeselectUnit()
    {
        game_manager.map_controller.ResetFields();

        selected_hex = null;
        selected_unit = null;

        SwitchActionMap("BasicHandler");
    }

    public void DeselectTargetableAbility()
    {
        game_manager.map_controller.ResetFields();

        targetable_ability = null;
        key_code = KeyCode.None;

        SwitchActionMap("SelectedUnitHandler");
    }

    private void OnAbilityPress(KeyCode _key_code)
    {
        Ability ability = selected_unit.GetBehaviour<Ability>(_key_code) as Ability;
        if (!selected_unit.IsWork() && ability != null && /*ability.HasCooldownExpired()*/ selected_unit.class_type == game_manager.game.class_on_turn)
        {
            game_manager.map_controller.ResetFields();
            if (ability is InstantleAbility)
            {
                NetInstantAbility request = new NetInstantAbility()
                {
                    unit_id = selected_unit.id,
                    col = selected_hex.coordinates.x,
                    row = selected_hex.coordinates.y,
                    key_code = _key_code
                };

                Sender.SendToServer_Reliable(request);

            }else if(ability is TargetableAbility targetable)
            {
                targetable_ability = targetable;
                key_code = _key_code;

                game_manager.map_controller.MarkAbilityMoves(ability, selected_hex);

                SwitchActionMap("TargetableAbilityHandler");
            }
        }
    }
}