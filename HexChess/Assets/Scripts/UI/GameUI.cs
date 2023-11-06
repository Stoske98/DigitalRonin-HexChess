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
    public Animator animator;
    [Header("Player Turn")]
    public Sprite light_turn;
    public Sprite dark_turn;
    public Image player_turn_image;

    [Header("Unit")]
    public TMP_Text damage;
    public TMP_Text health;
    public GameObject prefab_receive_damage_pop_up_txt;
    public List<AbilityController> abilities_controller = new List<AbilityController>();
    public Image unit_image;
    public GameObject image_object;

    [Header("Shards")]
    public TMP_Text shards;
    public List<UpgradeUnitUI> upgrades_unit_ui = new List<UpgradeUnitUI>();

    [Header("Interactive Bar")]
    public InteractiveBar interactive_bar;

    [Header("Log Bar")]
    public LogBar log_bar;


    [Header("Challenge Royale")]
    public TMP_Text ch_counger;
    public GameObject on_turn_object;

    [Header("Disconnect Panel")]
    public GameObject player_disconnect_from_game;


    [Header("Winner")]
    public Image winner;
    public Sprite light_winner;
    public Sprite dark_winner;


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
        game.game_events.OnChangeUnitData_Global += UpdateUnit;

        player_input_handler.OnSelectUnit += OnSelectUnit;
        player_input_handler.OnDeselectUnit += OnDeselectUnit;

        ChallengeRoyaleGame ch_game = game as ChallengeRoyaleGame;
        if (ch_game != null)
            ch_game.game_events.OnShardChanges_Global?.Invoke(ch_game);

        foreach (var upgrades in upgrades_unit_ui)
            upgrades.SetUp(ch_game.shard_controller);

    }
    public void UpdateUnit(Unit unit)
    {
        if (player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            OnSelectUnit(unit);
        }
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
        if (player_input_handler.GetSelectedUnit() != null && unit == player_input_handler.GetSelectedUnit())
        {
            OnSelectUnit(unit);
        }
    }

    private void OnUnitReceiveDamage(Unit unit, Damage damage, Hex hex)
    {
        GameObject damage_pop_up = Instantiate(prefab_receive_damage_pop_up_txt, unit.game_object.transform.position + Vector3.up * 2, Quaternion.identity);
        Vector3 directionToTarget = GameManager.Instance.map_controller.cm.gameObject.transform.position - damage_pop_up.transform.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        damage_pop_up.transform.rotation = targetRotation;

        damage_pop_up.GetComponentInChildren<TextMeshProUGUI>().text = damage.amount.ToString();
        damage_pop_up.transform.LeanMoveLocalY(3, 2);
        damage_pop_up.transform.LeanScale(Vector3.one * 2, 1.5f).setEase(LeanTweenType.easeInOutCubic);

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
        if(unit.real)
        {
            if (unit.class_type == ClassType.Light)
                log_bar.gralls_controller.SetCounter(unit.class_type, GameManager.Instance.game.death_light);
            else
                log_bar.gralls_controller.SetCounter(unit.class_type, GameManager.Instance.game.death_dark);
        }
    }

    public IEnumerator FinishDeathAnimation(Unit unit, float time)
    {
        yield return new WaitForSeconds(time);
        unit.animator.enabled = false;
        IObject.ObjectVisibility(unit, Visibility.NONE);

        unit.game_object.transform.position = new Vector3(100, 100, 100);
    }

    private void OnSelectUnit(Unit unit)
    {
        unit_image.sprite = unit.sprite;
        unit.health_bar_controller.ShowHealthBar();
        damage.text = unit.stats.damage.ToString();
        health.text = unit.stats.current_health.ToString();

        SetUpAbilities(unit);
    }
    private void OnDeselectUnit()
    {
        ClassType player_class_type = NetworkManager.Instance.player.data.class_type;
        List<IObject> objects = GameManager.Instance.game.object_manager.objects;
        foreach (IObject obj in objects)
        {
            if(obj is Unit unit)
            {
                if (unit != null && unit.class_type == player_class_type && unit.unit_type == UnitType.King)
                {
                    unit_image.sprite = unit.sprite;
                    damage.text = unit.stats.damage.ToString();
                    health.text = unit.stats.current_health.ToString();
                    SetUpAbilities(unit);
                    break;
                }
            }
        }
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
        GameManager game_manager = GameManager.Instance;
        Hex unit_hex = game_manager.game.map.GetHex(selected_unit);
        if (selected_unit != null && !selected_unit.IsDead() && unit_hex != null)
        {
            player_input_handler.SetSelectedHex(unit_hex);
            if (!selected_unit.IsWork() && selected_unit.class_type == game_manager.game.class_on_turn)
                game_manager.map_controller.MarkMovementAndAttackFields(game_manager.game, selected_unit, unit_hex);
        }
        else
            player_input_handler.DeselectUnit();
    }

    public void SetShards(ChallengeRoyaleGame ch_game)
    {
        if(NetworkManager.Instance.player.data.class_type == ClassType.Light)
            shards.text = ch_game.shard_controller.light_shards.ToString();
        else
            shards.text = ch_game.shard_controller.dark_shards.ToString();
    }

    private void OnClassUpgrade(ClassLevelController level_controller, ClassType class_type, UnitType unit_type_to_upgrade)
    {
        foreach (var upgrade in upgrades_unit_ui)
        {
            if (upgrade.unit_type == unit_type_to_upgrade)
            {
                if(NetworkManager.Instance.player.data.class_type == class_type)
                    upgrade.Upgrade(level_controller.level);

                Unit selected_unit = player_input_handler.GetSelectedUnit();
                if (selected_unit != null && unit_type_to_upgrade == selected_unit.unit_type)
                    OnSelectUnit(selected_unit);

                SetShards(GameManager.Instance.game as ChallengeRoyaleGame);
                break;
            }
        }
    }
    public void SetAWinner(ClassType class_type)
    {
        if(class_type == ClassType.Light)
            winner.sprite = light_winner;
        else
            winner.sprite = dark_winner;

        animator.Play("Win");
    }
    public void ActiveChallengeRoyale(bool just_end = false)
    {
        if (!just_end)
            animator.Play("ChallengeRoyale");
        else
            animator.Play("ChallengeRoyale", 0 ,1.0f);
    }

    public void SetChallengeRoyaleMove(int move)
    {
        ch_counger.text = move.ToString();
    }
    public void OnHoverUpgradeClassUI(RectTransform rect)
    {
      /*  LeanTween.value(gameObject, rect.sizeDelta.x, 375, 0.6f)
            .setOnUpdate((float width) => UpdateWidth(rect, width))
            .setEase(LeanTweenType.easeInOutCubic);*/
    }

    public void OnUnhoverUpgradeClassUI(RectTransform rect)
    {
      /*  LeanTween.value(gameObject, rect.sizeDelta.x, 98, 0.6f)
            .setOnUpdate((float width) => UpdateWidth(rect, width))
            .setEase(LeanTweenType.easeInOutCubic);*/
    }

    private void UpdateWidth(RectTransform rectTransform, float width)
    {
        // Update the RectTransform's width during the animation
        Vector2 newSize = rectTransform.sizeDelta;
        newSize.x = width;
        rectTransform.sizeDelta = newSize;
    }

}
