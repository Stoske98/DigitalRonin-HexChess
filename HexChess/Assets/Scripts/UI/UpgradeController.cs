using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeUnitUI : MonoBehaviour
{
    public UnitType unit_type;
    public List<TMP_Text> list_txt = new List<TMP_Text>();
    public Button button;

    public void SetUp(ShardController controller)
    {
        foreach (var level_controller in controller.class_level_controller) 
        {
            if(level_controller.unit_type == unit_type && level_controller.class_type == NetworkManager.Instance.player.player_data.class_type)
            {
                list_txt[0].gameObject.GetComponentInChildren<Image>().enabled = true;
                list_txt[1].text = level_controller.cost_to_upgrade[0].ToString();
                list_txt[2].text = level_controller.cost_to_upgrade[1].ToString();
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
            list_txt[0].gameObject.GetComponentInChildren<Image>().enabled = true;
            list_txt[1].gameObject.GetComponentInChildren<Image>().enabled = false;
            list_txt[2].gameObject.GetComponentInChildren<Image>().enabled = false;

        }
        else if (level == 2)
        {
            list_txt[1].gameObject.GetComponentInChildren<Image>().enabled = true;
            list_txt[2].gameObject.GetComponentInChildren<Image>().enabled = false;
        }
        else
        {
            list_txt[1].gameObject.GetComponentInChildren<Image>().enabled = true;
            list_txt[2].gameObject.GetComponentInChildren<Image>().enabled = true;
        }
    }

    private void OnClick()
    {
        ChallengeRoyaleGame ch_game = GameManager.Instance.game as ChallengeRoyaleGame;
        if (ch_game != null && !ch_game.object_manager.IsObjectsWorking() && NetworkManager.Instance.player.player_data.class_type == ch_game.class_on_turn && ch_game.shard_controller.CanClassBeUpgraded(NetworkManager.Instance.player.player_data.class_type, unit_type))
        {
            NetUpgradeClass request = new NetUpgradeClass()
            {
                class_type = NetworkManager.Instance.player.player_data.class_type,
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
