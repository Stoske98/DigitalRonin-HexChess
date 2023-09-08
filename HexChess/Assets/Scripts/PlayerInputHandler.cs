using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public GameManager game_manager;

    public PlayerInput player_input_controller;
    private Hex selected_hex { get; set; }
    private Unit selected_unit { get; set; }
    private TargetableAbility targetable_ability { get; set; }
    private KeyCode key_code = KeyCode.None;


    public Action<Unit> OnSelectUnit;
    public Action OnDeselectUnit;

    public void SwitchActionMap(string new_action_map)
    {
        // Debug.Log("Current map DISABLE: " + player_input_controller.currentActionMap.name);
        player_input_controller.currentActionMap?.Disable();
        player_input_controller.currentActionMap = player_input_controller.actions.FindActionMap(new_action_map);
        player_input_controller.currentActionMap?.Enable();
       //  Debug.Log("Current map ENABLE: " + player_input_controller.currentActionMap.name);
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
            if (_hex != null && _unit != null && !EventSystem.current.IsPointerOverGameObject())
            {
                game_manager.map_controller.ResetFields();
                SelectUnit(_unit, _hex);

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
            if (_hex != null && _unit != null && _unit != selected_unit && !EventSystem.current.IsPointerOverGameObject())
            {
                game_manager.map_controller.ResetFields();
                SelectUnit(_unit, _hex);
                return;
            }
            //MOVE           
            if (_hex != null && _hex.IsWalkable() && !selected_unit.IsWork() && !Stun.IsStuned(selected_unit) && selected_unit.class_type == game_manager.game.class_on_turn && !EventSystem.current.IsPointerOverGameObject())
            {
                MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>();
                if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(selected_hex).Contains(_hex))
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
                if (!selected_unit.IsWork() && !Stun.IsStuned(selected_unit) && !Disarm.IsDissarmed(selected_unit) && _unit.class_type != selected_unit.class_type)
                {
                    AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>();
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
        if (value.started)
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
    public void OnSelectSingleTargetAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();

            if (_hex != null && targetable_ability.GetAbilityMoves(selected_hex).Contains(_hex) && !EventSystem.current.IsPointerOverGameObject())
            {
                NetSingleTargetAbilility request = new NetSingleTargetAbilility()
                {
                    unit_id = selected_unit.id,
                    col = selected_hex.coordinates.x,
                    row = selected_hex.coordinates.y,
                    desired_col = _hex.coordinates.x,
                    desired_row = _hex.coordinates.y,
                    key_code = key_code
                };
                Sender.SendToServer_Reliable(request);

                DeselectTargetAbilityHandler();
            }
        }
    }
    public void OnDeselectSingleTargetAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            DeselectTargetAbilityHandler();

            if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
                game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, selected_hex);
        }
    }
    public void OnSelectMultipleTargetsAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Hex _hex = game_manager.map_controller.GetCurrentHex();

            ITargetMultipleHexes targetable_multiple_hexes = targetable_ability as ITargetMultipleHexes;
            if (_hex != null && targetable_ability.GetAbilityMoves(selected_hex).Contains(_hex) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (targetable_multiple_hexes.targetable_hexes.Count < targetable_multiple_hexes.max_hexes)
                {
                    if (!targetable_multiple_hexes.targetable_hexes.Contains(_hex))
                        targetable_multiple_hexes.targetable_hexes.Add(_hex);
                    else
                        targetable_multiple_hexes.targetable_hexes.Remove(_hex);
                } else
                {
                    if (targetable_multiple_hexes.targetable_hexes.Contains(_hex))
                        targetable_multiple_hexes.targetable_hexes.Remove(_hex);
                }
            }
        }
    }
    public void OnDeselectMultipleTargetsAbilityHandler(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            DeselectMultipleTargetsAbility();           
        }
    }
    private void DeselectMultipleTargetsAbility()
    {
        ITargetMultipleHexes multiple_targets = targetable_ability as ITargetMultipleHexes;
        if ((multiple_targets.has_condition && multiple_targets.max_hexes == multiple_targets.targetable_hexes.Count) || (!multiple_targets.has_condition && multiple_targets.targetable_hexes.Count > 0))
        {
            List<Vector2Int> hexes_coordinates = new List<Vector2Int>();
            foreach (var hex in multiple_targets.targetable_hexes)
                hexes_coordinates.Add(hex.coordinates);

            NetMultipeTargetsAbilility request = new NetMultipeTargetsAbilility()
            {
                unit_id = selected_unit.id,
                col = selected_hex.coordinates.x,
                row = selected_hex.coordinates.y,
                hexes_coordiantes = hexes_coordinates,
                key_code = key_code
            };
            Sender.SendToServer_Reliable(request);

            DeselectTargetAbilityHandler();
        }
        else
        {
            DeselectTargetAbilityHandler();

            if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
                game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, selected_hex);
        }
        multiple_targets.targetable_hexes.Clear();
    }
    public void SelectUnit(Unit _unit, Hex _hex)
    {
        selected_unit?.health_bar_controller.HideHealthBar();

        selected_hex = _hex;
        selected_unit = _unit;

        if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
            game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, _hex);

        selected_unit.health_bar_controller.ShowHealthBar();
        OnSelectUnit?.Invoke(selected_unit);
    }

    public void DeselectUnit()
    {
        game_manager.map_controller.ResetFields();
        selected_unit?.health_bar_controller.HideHealthBar();

        selected_hex = null;
        selected_unit = null;

        OnDeselectUnit?.Invoke();
        SwitchActionMap("BasicHandler");
    }

    private void DeselectTargetAbilityHandler()
    {
        game_manager.map_controller.ResetFields();

        targetable_ability = null;
        key_code = KeyCode.None;

        SwitchActionMap("SelectedUnitHandler");
    }

    private void OnAbilityPress(KeyCode _key_code)
    {
        Ability ability = selected_unit.GetBehaviour<Ability>(_key_code);
        if (!selected_unit.IsWork() && !Stun.IsStuned(selected_unit) && ability != null && ability.HasCooldownExpired() && selected_unit.class_type == game_manager.game.class_on_turn)
        {
          
            if (ability is InstantleAbility instant_ability)
            {
                if (instant_ability.GetAbilityMoves(selected_hex).Count > 0)
                {
                    game_manager.map_controller.ResetFields();
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = _key_code
                    };

                    Sender.SendToServer_Reliable(request);

                }
                else { Debug.Log("There is no moves for ability !!!"); }
            }
            else if (ability is TargetableAbility targetable)
            {
                if (targetable.GetAbilityMoves(selected_hex).Count > 0)
                {
                    game_manager.map_controller.ResetFields();
                    targetable_ability = targetable;
                    key_code = _key_code;

                    game_manager.map_controller.MarkAbilityMoves(ability, selected_hex);

                    if (ability is ITargetableSingleHex)
                        SwitchActionMap("SingleTargetAbilityHandler");
                    else
                        SwitchActionMap("MultipleTargetsAbilityHandler");

                }
                else { Debug.Log("There is no moves for ability !!!"); }
            }
        }
    }
    public void SetSelectedUnit(Unit unit)
    {
        selected_unit = unit;
    }
    public void SetSelectedHex(Hex hex)
    {
        selected_hex = hex;
    }

    public Unit GetSelectedUnit()
    {
        return selected_unit;
    }
    public Hex GetSelectedHex()
    {
        return selected_hex;
    }
}