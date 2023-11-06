using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    public List<Sprite> light_sprites;
    public List<Sprite> dark_sprites;

    [Header("Attack bar")]
    public GameObject attack_bar;
    public Image attack_actor_1;
    public Image attack_actor_2;

    [Header("Movement bar")]
    public GameObject movement_bar;
    public Image movement_actor_1;

    [Header("Ability bar")]
    public GameObject ability_bar;
    public Image ability_actor_1;
    public Image ability_actor_2;
    public Image spell;

    
    public void SetAttacknBar(Unit actor1, Unit actor2)
    {
        ResetBar();
        attack_actor_1.sprite = GetSprite(actor1);
        attack_actor_2.sprite = GetSprite(actor2);
        attack_bar.SetActive(true);
    }

    public void SetMovementnBar(Unit actor1)
    {
        ResetBar();
        movement_actor_1.sprite = GetSprite(actor1);
        movement_bar.SetActive(true);
    }

    public void SetSpellBar(Unit actor1, Ability ability, Unit actor2 = null)
    {
        ResetBar();
        ability_actor_1.sprite = GetSprite(actor1);
        spell.sprite = ability.sprite;

        if(actor2 == null)
            ability_actor_2.enabled = false;
        else
        {
            ability_actor_2.sprite = GetSprite(actor2);
            ability_actor_2.enabled = true;
        }

        ability_bar.SetActive(true);
    }
    private Sprite GetSprite(Unit unit)
    {
        if(unit.class_type == ClassType.Light)
            return light_sprites[(int)unit.unit_type];
        else
            return dark_sprites[(int)unit.unit_type];
    }
    public void ResetBar()
    {
        attack_bar.SetActive(false);
        movement_bar.SetActive(false);
        ability_bar.SetActive(false);
    }
}
