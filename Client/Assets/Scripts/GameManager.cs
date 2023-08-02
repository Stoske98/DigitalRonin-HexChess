using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Playables;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region GameManager Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log("Game Manager instance already exist, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    #endregion
    [JsonConverter(typeof(CustomConverters.GameConverter))] public Game game;
    public MapController map_controller;

    Hex selected_hex;
    public Unit selected_unit;

    public Action<Hex> OnSelectUnit;

    private AbilityBehaviour current_ability = null;
    private KeyCode targetable_ability_key_code = KeyCode.None;

    //private PlayerInputHandler input_handler;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //input_handler = new BasicHandler();

        /*  Map map = new ChallengeRoyaleMap(4,4,1.05f,1);
          game = new ChallengeRoyaleGame(map, 30, 10);
          map_controller.SetMap(map);

          string json = NetworkManager.Serialize(game);
          //Debug.Log(json);
          File.WriteAllText("ChallengeRoyaleGame.json", json);*/

        /* game = NetworkManager.Deserialize<ChallengeRoyaleGame>(File.ReadAllText("ChallengeRoyaleGame.json"));
         string json = NetworkManager.Serialize(game);

         foreach (var obj in game.objects)
             if (obj is Unit unit)
             {
                 game.units.Add(unit);
                 unit.RegisterEvents();
                 Spawner.CreateUnitGameObjects(unit);
             }

         foreach (Hex hex in game.map.hexes)
         {
             hex.SetNeighbors(game.map);
             hex.hex_mesh = hex.game_object.GetComponent<MeshRenderer>();
             hex.SetMaterial(map_controller.field_material);
         }


         map_controller.SetMap(game.map);

         json = NetworkManager.Serialize(game);
         File.WriteAllText("NewChallengeRoyaleGame.json", json);*/


    }

    public void Save()
    {
        Debug.Log("GAME SAVED");
        string json = NetworkManager.Serialize(game);
        File.WriteAllText("ChallengeRoyaleGame.json", json);
    }
    void Update()
    {
        game?.Update();
        if (Input.GetKeyDown("space"))
        {
            foreach (var item in game.GetMapHexes())
            {
                if (!item.IsWalkable())
                    item.SetColor(Color.black);
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
            Save();
       // OnSelect();
    }

   /* private void OnSelect()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(map_controller.GetCurrentHex() != null && map_controller.GetCurrentHex() != selected_hex)
            {
                if (selected_unit != null && selected_unit.class_type == game.class_on_turn && current_ability != null)
                {
                    if(current_ability is TargetableAbility targetable)
                        if(targetable.GetAbilityMoves(selected_hex).Contains(map_controller.GetCurrentHex()))
                        {
                            NetTargetableAbilility request = new NetTargetableAbilility()
                            {
                                unit_id = selected_unit.id,
                                col = selected_hex.coordinates.x,
                                row = selected_hex.coordinates.y,
                                desired_col = map_controller.GetCurrentHex().coordinates.x,
                                desired_row = map_controller.GetCurrentHex().coordinates.y,
                                key_code = targetable_ability_key_code
                            };
                            Sender.SendToServer_Reliable(request);
                            map_controller.ResetFields();
                        }
                }
                else
                {
                    if (map_controller.GetCurrentHex().GetUnit() != null)
                    {
                        selected_hex = map_controller.GetCurrentHex();
                        selected_unit = selected_hex.GetUnit();
                        //OnSelectUnit.Invoke(selected_hex);
                        map_controller.ResetFields();
                        if(!Stun.IsStuned(selected_unit.ccs) && selected_unit.class_type == game.class_on_turn)
                        {
                            MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
                            AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>() as AttackBehaviour;
                            
                            if(movement_behaviour != null)
                                map_controller.MarkAvailableMoves(movement_behaviour, selected_hex);
                            if (!Disarm.IsDissarmed(selected_unit.ccs) && attack_behaviour != null)
                                map_controller.MarkAttackMoves(attack_behaviour, selected_hex);
                        }
                    }
                    else if (selected_unit != null && selected_unit.class_type == game.class_on_turn)
                    {
                        MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
                        if (movement_behaviour != null && movement_behaviour.GetAvailableMoves(selected_hex).Contains(map_controller.GetCurrentHex()))
                        {
                            NetMove request = new NetMove()
                            {
                                unit_id = selected_unit.id,
                                col = selected_hex.coordinates.x,
                                row = selected_hex.coordinates.y,
                                desired_col = map_controller.GetCurrentHex().coordinates.x,
                                desired_row = map_controller.GetCurrentHex().coordinates.y
                            };
                            Sender.SendToServer_Reliable(request);

                            selected_hex = null;
                            map_controller.ResetFields();
                        }
                    }
                }
            }            
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (map_controller.GetCurrentHex() != null && map_controller.GetCurrentHex().GetUnit() != null)
            {
                if(selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_hex.GetUnit() == selected_unit && selected_unit.class_type != map_controller.GetCurrentHex().GetUnit().class_type)
                {
                    AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>() as AttackBehaviour;
                    if (attack_behaviour != null && attack_behaviour.GetAttackMoves(selected_hex).Contains(map_controller.GetCurrentHex()))
                    {
                        Unit target = map_controller.GetCurrentHex().GetUnit();
                        NetAttack request = new NetAttack()
                        {
                            attacker_id = selected_unit.id,
                            attacker_col = selected_hex.coordinates.x,
                            attacker_row = selected_hex.coordinates.y,
                            target_id = target.id,
                            target_col = map_controller.GetCurrentHex().coordinates.x,
                            target_row = map_controller.GetCurrentHex().coordinates.y
                        };
                        Sender.SendToServer_Reliable(request);

                        map_controller.ResetFields();
                    }
                }
            }

            if (selected_unit != null)
            {
                current_ability = null;
                targetable_ability_key_code = KeyCode.None;
            }
        }
        if(Input.GetKeyDown(KeyCode.Q) && selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_unit == selected_hex.GetUnit())
        {
            Ability ability = selected_unit.GetBehaviour<Ability>(KeyCode.Q) as Ability;
           // if (ability != null /*&& ability.HasCooldownExpired()*///)/
          /*  {
                map_controller.ResetFields();
                map_controller.MarkAbilityMoves(ability, selected_hex);
                if (ability is InstantleAbility instant)
                {
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = KeyCode.Q
                    };
                    Sender.SendToServer_Reliable(request);
                }
                else if (ability is TargetableAbility targetable)
                {
                    current_ability = targetable;
                    targetable_ability_key_code = KeyCode.Q;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W) && selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_unit == selected_hex.GetUnit())
        {
            Ability ability = selected_unit.GetBehaviour<Ability>(KeyCode.W) as Ability;
            if (ability != null /*&& ability.HasCooldownExpired()*///)
         /*   {
                map_controller.ResetFields();
                map_controller.MarkAbilityMoves(ability, selected_hex);
                if (ability is InstantleAbility instant)
                {
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = KeyCode.W
                    };
                    Sender.SendToServer_Reliable(request);
                }
                else if (ability is TargetableAbility targetable)
                {
                    current_ability = targetable;
                    targetable_ability_key_code = KeyCode.W;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_unit == selected_hex.GetUnit())
        {
            Ability ability = selected_unit.GetBehaviour<Ability>(KeyCode.E) as Ability;
           if (ability != null /*&& ability.HasCooldownExpired()*///)
         /*   {
                map_controller.ResetFields();
                map_controller.MarkAbilityMoves(ability, selected_hex);
                if (ability is InstantleAbility instant)
                {
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = KeyCode.E
                    };
                    Sender.SendToServer_Reliable(request);
                }
                else if (ability is TargetableAbility targetable)
                {
                    current_ability = targetable;
                    targetable_ability_key_code = KeyCode.E;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_unit == selected_hex.GetUnit())
        {
            Ability ability = selected_unit.GetBehaviour<Ability>(KeyCode.R) as Ability;
            if (ability != null /*&& ability.HasCooldownExpired()*//// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
           /* {
                map_controller.ResetFields();
                map_controller.MarkAbilityMoves(ability, selected_hex);
                if (ability is InstantleAbility instant)
                {
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = KeyCode.R
                    };
                    Sender.SendToServer_Reliable(request);
                }
                else if (ability is TargetableAbility targetable)
                {
                    current_ability = targetable;
                    targetable_ability_key_code = KeyCode.R;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S) && selected_unit != null && selected_unit.class_type == game.class_on_turn && selected_unit == selected_hex.GetUnit())
        {
            Ability ability = selected_unit.GetBehaviour<Ability>(KeyCode.S) as Ability;
            if (ability != null /*&& ability.HasCooldownExpired()*///)
          /*  {
                map_controller.ResetFields();
                map_controller.MarkAbilityMoves(ability, selected_hex);
                if (ability is InstantleAbility instant)
                {
                    NetInstantAbility request = new NetInstantAbility()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        key_code = KeyCode.S
                    };
                    Sender.SendToServer_Reliable(request);
                }
                else if (ability is TargetableAbility targetable)
                {
                    current_ability = targetable;
                    targetable_ability_key_code = KeyCode.S;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        map_controller.ResetFields();

    }*/

   /* public void OnSelect(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.LEFT_CLICK);
    }
    public void OnDeselect(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.RIGHT_CLICK);
    }
    public void OnSpecialAbility(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.SPECIAL_ABILITY);
    }
    public void OnAbility1(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.ABILITY_1);
    }
    public void OnAbility2(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.ABILITY_2);
    }
    public void OnAbility3(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.ABILITY_3);
    }
    public void OnAbility4(InputAction.CallbackContext value)
    {
        if (value.started)
            input_handler.ForwardInput(PlayerInput.ABILITY_4);
    }

    public void ChangePlayerInput(PlayerInputHandler new_handler)
    {
        input_handler = new_handler;
    }*/
}

/*public interface PlayerInputHandler
{
    public void ForwardInput(PlayerInput input);
}

public class BasicHandler : PlayerInputHandler
{
    public void ForwardInput(PlayerInput input)
    {
        Hex _hex = GameManager.Instance.map_controller.GetCurrentHex();
        Unit _unit = _hex?.GetUnit();
        switch (input) 
        {
            case PlayerInput.LEFT_CLICK:
                if (_hex != null && _unit != null)
                    SelectUnit(_hex, _unit);
                break;
            case PlayerInput.RIGHT_CLICK: 
                break;

            default:
                break;
        }
    }

    private void SelectUnit(Hex _hex, Unit _unit)
    {
        SelectedUnitHandler selected_unit_handler = new SelectedUnitHandler()
        {
            selected_hex = _hex,
            selected_unit = _unit,
        };
        if(!_unit.IsWork() && _unit.class_type == GameManager.Instance.game.class_on_turn)
            GameManager.Instance.MarkFields(_unit,_hex);

        GameManager.Instance.OnSelectUnit?.Invoke(_hex);
        GameManager.Instance.ChangePlayerInput(selected_unit_handler);
    }
}
public class TargetableAbilityHandler : PlayerInputHandler
{
    public TargetableAbility ability { get; set; }
    public Hex selected_hex { get; set; }
    public Unit selected_unit { get; set; }
    public KeyCode key_code { get; set; }
    public void ForwardInput(PlayerInput input)
    {
        Hex _hex = GameManager.Instance.map_controller.GetCurrentHex();
        SelectedUnitHandler selected_unit_handler = new SelectedUnitHandler()
        {
            selected_hex = selected_hex,
            selected_unit = selected_unit,
        };

        GameManager.Instance.map_controller.ResetFields();
        switch (input)
        {
            case PlayerInput.LEFT_CLICK:
                if (_hex != null && ability.GetAbilityMoves(selected_hex).Contains(_hex))
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

                    GameManager.Instance.ChangePlayerInput(selected_unit_handler);
                }
                break;
            case PlayerInput.RIGHT_CLICK:

                GameManager.Instance.ChangePlayerInput(selected_unit_handler);
                break;

            default:
                break;
        }
    }
}
public class SelectedUnitHandler : PlayerInputHandler
{
    public Hex selected_hex { get; set; }
    public Unit selected_unit { get; set; }

    public void ForwardInput(PlayerInput input)
    {
        Hex _hex = GameManager.Instance.map_controller.GetCurrentHex();
        Unit _unit = _hex?.GetUnit();
        switch (input)
        {
            case PlayerInput.LEFT_CLICK:
                //SELECT OTHER UNITS
                if (_hex != null && _unit != null && _unit != selected_unit)
                {
                    SelectUnit(_hex, _unit);
                    GameManager.Instance.OnSelectUnit?.Invoke(_hex);

                    if (!_unit.IsWork() && _unit.class_type == GameManager.Instance.game.class_on_turn)
                        GameManager.Instance.MarkFields(_unit, _hex);
                    else
                        GameManager.Instance.map_controller.ResetFields();

                    return;
                }
                //MOVE
                MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>() as MovementBehaviour;
                if(_hex != null && _hex.IsWalkable() && !selected_unit.IsWork()  && selected_unit.class_type == GameManager.Instance.game.class_on_turn 
                    && movement_behaviour != null && movement_behaviour.GetAvailableMoves(selected_hex).Contains(_hex))
                {
                    GameManager.Instance.map_controller.ResetFields();

                    NetMove request = new NetMove()
                    {
                        unit_id = selected_unit.id,
                        col = selected_hex.coordinates.x,
                        row = selected_hex.coordinates.y,
                        desired_col = _hex.coordinates.x,
                        desired_row = _hex.coordinates.y
                    };
                    Sender.SendToServer_Reliable(request);

                    return;
                }

                break;
            case PlayerInput.RIGHT_CLICK:

                GameManager.Instance.map_controller.ResetFields();
                //ATTACK
                AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>() as AttackBehaviour;
                if (_unit != null && !selected_unit.IsWork() && _unit != selected_unit && _unit.class_type != selected_unit.class_type 
                    && selected_unit.class_type == GameManager.Instance.game.class_on_turn
                    && attack_behaviour != null && attack_behaviour.GetAttackMoves(selected_hex).Contains(_hex))
                {
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

                    return;
                }
                //DESELECT UNIT

                GameManager.Instance.ChangePlayerInput(new BasicHandler());

                break;
            case PlayerInput.SPECIAL_ABILITY:
                OnAbilityPress(KeyCode.S);
                break; 
            case PlayerInput.ABILITY_1:
                OnAbilityPress(KeyCode.Q);
                break; 
            case PlayerInput.ABILITY_2:
                OnAbilityPress(KeyCode.W);
                break; 
            case PlayerInput.ABILITY_3:
                OnAbilityPress(KeyCode.E);
                break;
            case PlayerInput.ABILITY_4:
                OnAbilityPress(KeyCode.R);
                break;

            default:
                break;
        }
    }

    private void SelectUnit(Hex _hex, Unit _unit)
    {
        SelectedUnitHandler selected_unit_handler = new SelectedUnitHandler()
        {
            selected_hex = _hex,
            selected_unit = _unit,
        };

        GameManager.Instance.ChangePlayerInput(selected_unit_handler);
    }

    private void OnAbilityPress(KeyCode _key_code)
    {
        Ability ability = selected_unit.GetBehaviour<Ability>(_key_code) as Ability;
        if (!selected_unit.IsWork() && ability != null && /*ability.HasCooldownExpired()*/ /*selected_unit.class_type == GameManager.Instance.game.class_on_turn)
        {
            if (ability is InstantleAbility)
            {
                GameManager.Instance.map_controller.ResetFields();
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
                TargetableAbilityHandler ability_handler = new TargetableAbilityHandler()
                {
                    ability = targetable,
                    selected_hex = selected_hex,
                    selected_unit = selected_unit,
                    key_code= _key_code
                };

                GameManager.Instance.map_controller.MarkAbilityMoves(ability, selected_hex);
                GameManager.Instance.ChangePlayerInput(ability_handler);
            }
        }
    }
}

public enum PlayerInput
{
    LEFT_CLICK, RIGHT_CLICK, SPECIAL_ABILITY, ABILITY_1, ABILITY_2, ABILITY_3, ABILITY_4
}*/



