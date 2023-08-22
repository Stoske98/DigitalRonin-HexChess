using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameUI : MonoBehaviour
{
    #region GameUI Singleton
    private static GameUI _instance;
    public static GameUI Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log("Game UI instance already exist, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    #endregion
    public PlayerInputHandler player_input_handler;
    [Header("Player Turn")]
    public Sprite light_turn;
    public Sprite dark_turn;
    public Image player_turn_image;

    [Header("Unit")]
    public TMP_Text damage;
    public TMP_Text health;
    public List<AbilityController> abilities_controller = new List<AbilityController>();
    public Image unit_image;
    public GameObject image_object;

    [Header("Shards")]
    public TMP_Text shards;
    public List<UpgradeUnitUI> upgrades_unit_ui = new List<UpgradeUnitUI>();


    private void Awake()
    {
        Instance = this;
    }
    public void Subscribe(Game game)
    {
        game.game_events.OnEndPlayerTurn_Global += SetPlayerTurn;
        game.game_events.OnShardChanges_Global += SetShards;
        game.game_events.OnClassUpgraded_Global += OnClassUpgrade;
        game.game_events.OnUnitDeath_Global += OnUnitDeath;
        game.game_events.OnReceiveDamage_Global += OnUnitReceiveDamage;
        game.game_events.OnUseAbility_Global += OnUnitUseAbility;
        game.game_events.OnUpdateCooldown_Global += OnUpdateCooldown;

        player_input_handler.OnSelectUnit += OnSelectUnit;
        player_input_handler.OnDeselectUnit += OnDeselectUnit;

        ChallengeRoyaleGame ch_game = game as ChallengeRoyaleGame;
        if (ch_game != null)
            ch_game.game_events.OnShardChanges_Global?.Invoke(ch_game);

        foreach (var upgrades in upgrades_unit_ui)
            upgrades.SetUp(ch_game.shard_controller);

    }

    private void OnUpdateCooldown(Unit unit)
    {
        if (player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            OnSelectUnit(unit);
        }
    }

    private void OnUnitUseAbility(Unit unit)
    {
        if(player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            OnSelectUnit(unit);
        }
    }

    private void OnUnitReceiveDamage(Unit unit, Damage damage, Hex hex)
    {
        if (player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            OnSelectUnit(unit);
        }
    }

    private void OnUnitDeath(Unit unit)
    {
        StartCoroutine(FinishDeathAnimation(unit, 2));

        if (player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            player_input_handler.DeselectUnit();   
            OnDeselectUnit();
        }
    }

    public IEnumerator FinishDeathAnimation(Unit unit, float time)
    {
        yield return new WaitForSeconds(time);
        unit.animator.enabled = false;
        IObject.ObjectVisibility(unit, Visibility.NONE);
    }

    private void OnSelectUnit(Unit unit)
    {
        image_object.SetActive(true);
        unit_image.sprite = unit.sprite;
        damage.text = unit.stats.damage.ToString();
        health.text = unit.stats.current_health.ToString();
        SetUpAbilities(unit);
    }
    private void OnDeselectUnit()
    {
        image_object.SetActive(false);
    }

    private void SetUpAbilities(Unit unit)
    {
        List<AbilityBehaviour> ability_behaviours = new List<AbilityBehaviour>(); 

        foreach (var behaviour in unit.behaviours)
            if (behaviour is AbilityBehaviour ability_behaviour)
                ability_behaviours.Add(ability_behaviour);

        for (int i = 0; i < abilities_controller.Count; i++)
        {
            if(i > ability_behaviours.Count - 1)
            {
                abilities_controller[i].gameObject.SetActive(false);
                continue;
            }
            abilities_controller[i].SetAbility(ability_behaviours[i]);
            abilities_controller[i].gameObject.SetActive(true);

        }
    }
    public void SetPlayerTurn(ClassType class_type)
    {
        if(class_type == ClassType.Light)
            player_turn_image.sprite = light_turn;
        else
            player_turn_image.sprite = dark_turn;

        Unit selected_unit = player_input_handler.GetSelectedUnit();
        Hex selected_hex = player_input_handler.GetSelectedHex();
        GameManager game_manager = GameManager.Instance;
        if (selected_unit != null && !selected_unit.IsDead() && selected_hex != null)
        {
            if(!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
                game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, selected_hex);
        }
    }

    public void SetShards(ChallengeRoyaleGame ch_game)
    {
        if(NetworkManager.Instance.player.player_data.class_type == ClassType.Light)
            shards.text = ch_game.shard_controller.light_shards.ToString();
        else
            shards.text = ch_game.shard_controller.dark_shards.ToString();
    }

    private void OnClassUpgrade(ClassLevelController level_controller, UnitType unit_type_to_upgrade)
    {
        foreach (var upgrade in upgrades_unit_ui)
        {
            if (upgrade.unit_type == unit_type_to_upgrade)
            {
                upgrade.Upgrade(level_controller.level);

                Unit selected_unit = player_input_handler.GetSelectedUnit();
                if (selected_unit != null && unit_type_to_upgrade == selected_unit.unit_type)
                    OnSelectUnit(selected_unit);

                SetShards(GameManager.Instance.game as ChallengeRoyaleGame);
                break;
            }
        }
    }

    public void OnHoverUpgradeClassUI(RectTransform rect)
    {
        LeanTween.value(gameObject, rect.sizeDelta.x, 350, 0.6f)
            .setOnUpdate((float width) => UpdateWidth(rect, width))
            .setEase(LeanTweenType.easeInOutCubic);
    }

    public void OnUnhoverUpgradeClassUI(RectTransform rect)
    {
        LeanTween.value(gameObject, rect.sizeDelta.x, 98, 0.6f)
            .setOnUpdate((float width) => UpdateWidth(rect, width))
            .setEase(LeanTweenType.easeInOutCubic);
    }

    private void UpdateWidth(RectTransform rectTransform, float width)
    {
        // Update the RectTransform's width during the animation
        Vector2 newSize = rectTransform.sizeDelta;
        newSize.x = width;
        rectTransform.sizeDelta = newSize;
    }

}
