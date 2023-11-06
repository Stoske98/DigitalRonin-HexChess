using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrallsController : MonoBehaviour
{
    public TMP_Text light_counter;
    public List<Image> light_fires;
    public TMP_Text dark_counter;
    public List<Image> dark_fires;
    public void SetCounter(ClassType class_type, int number_of_death)
    {
        if(class_type == ClassType.Light)
        {
            light_counter.text = number_of_death.ToString();
            if (number_of_death <= 6)
            {
                light_fires[0].enabled = true;
                light_fires[1].enabled = false;
                light_fires[2].enabled = false;
            }
            else if (number_of_death <= 9)
            {
                light_fires[0].enabled = false;
                light_fires[1].enabled = true;
                light_fires[2].enabled = false;
            }
            else
            {
                light_fires[0].enabled = false;
                light_fires[1].enabled = false;
                light_fires[2].enabled = true;
            }
        }
        else
        {
            dark_counter.text = number_of_death.ToString();
            if (number_of_death <= 6)
            {
                dark_fires[0].enabled = true;
                dark_fires[1].enabled = false;
                dark_fires[2].enabled = false;
            }
            else if (number_of_death <= 9)
            {
                dark_fires[0].enabled = false;
                dark_fires[1].enabled = true;
                dark_fires[2].enabled = false;
            }
            else
            {
                dark_fires[0].enabled = false;
                dark_fires[1].enabled = false;
                dark_fires[2].enabled = true;
            }
        }
    }
}
