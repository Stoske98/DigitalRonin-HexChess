using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityController : MonoBehaviour
{
    public Image ability_image;
    public Image ability_cooldown;
    public TMP_Text cooldown_txt;
    public AbilityBehaviour ability;

    public void SetAbility(AbilityBehaviour ability_behaviour)
    {
        ability = ability_behaviour;
        if (ability_behaviour is PassiveAbility passive_ability)
        {
            ability_image.sprite = ability_behaviour.sprite;
            ability_cooldown.fillAmount = 0;
            cooldown_txt.text = "";
        }
        else if(ability_behaviour is Ability ability)
        {
            ability_image.sprite = ability_behaviour.sprite;

            if (ability.ability_data.current_cooldown == 0)
            {
                ability_cooldown.fillAmount = 0;
                cooldown_txt.text = "";
            }
            else
            {
                ability_cooldown.fillAmount = (float)ability.ability_data.current_cooldown / (float)ability.ability_data.max_cooldown;
                cooldown_txt.text = ability.ability_data.current_cooldown.ToString();
            }
        }
    }

}
