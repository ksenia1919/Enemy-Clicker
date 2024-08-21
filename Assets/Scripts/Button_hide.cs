using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_hide : MonoBehaviour
{
    public GameObject fightObject;
    public GameObject upObject;
    public GameObject mapObject;
    public void OnButtonClick()
    {
        string buttonTag = gameObject.tag;

        if (buttonTag == "FightBut")
        {
            fightObject.SetActive(true);
            upObject.SetActive(false);
            mapObject.SetActive(false);
        }
        else if (buttonTag == "UpBut")
        {
            upObject.SetActive(true);
            fightObject.SetActive(false);
            mapObject.SetActive(false);
        }
        else if (buttonTag == "MapBut")
        {
            mapObject.SetActive(true);
            upObject.SetActive(false);
            fightObject.SetActive(false);
        }

    }
}
