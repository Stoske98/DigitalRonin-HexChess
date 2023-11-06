using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeUnitUI : MonoBehaviour
{
    public UnitType unit_type;
    public TMP_Text form_text;
    public TMP_Text cost_to_upgrade;
    public List<Image> star_images = new List<Image>();
    public Button button;
    private List<int> cost_per_level = new List<int>();
    private string form_1 = "Figure form";
    private string form_2 = "Ascended f.";
    private string form_3 = "Prime form";

    public void SetUp(ShardController controller)
    {
        foreach (var level_controller in controller.class_level_controller) 
        {
            if(level_controller.unit_type == unit_type && level_controller.class_type == NetworkManager.Instance.player.data.class_type)
            {
                foreach (int cost in level_controller.cost_to_upgrade)
                    cost_per_level.Add(cost);
                button.onClick.AddListener(OnClick);
                Upgrade(level_controller.level);
                break;
            }
        }
    }

    public void Upgrade(int level)
    {
        if (level == 1)
        {
            form_text.text = form_1;
            cost_to_upgrade.text = cost_per_level[level-1].ToString();
            star_images[level - 1].enabled = true;

        }
        else if (level == 2)
        {
            form_text.text = form_2;
            cost_to_upgrade.text = cost_per_level[level-1].ToString();
            star_images[level - 1].enabled = true;
            star_images[level - 2].enabled = true;
        }
        else
        {
            form_text.text = form_3;
            button.gameObject.SetActive(false);
            cost_to_upgrade.text = "";
            star_images[level - 1].enabled = true;
            star_images[level - 2].enabled = true;
            star_images[level - 3].enabled = true;
        }
    }

    private void OnClick()
    {
        ChallengeRoyaleGame ch_game = GameManager.Instance.game as ChallengeRoyaleGame;
        if (ch_game != null && !ch_game.object_manager.IsObjectsWorking() && NetworkManager.Instance.player.data.class_type == ch_game.class_on_turn && ch_game.shard_controller.CanClassBeUpgraded(NetworkManager.Instance.player.data.class_type, unit_type))
        {
            NetUpgradeClass request = new NetUpgradeClass()
            {
                class_type = NetworkManager.Instance.player.data.class_type,
                unit_type_to_upgrade = unit_type,
            };
            Sender.SendToServer_Reliable(request);
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
}
