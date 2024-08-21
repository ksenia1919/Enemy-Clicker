using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShop : MonoBehaviour
{

    public SetingsButton setingsButton;
    public void StartUpdate()
    {
        setingsButton = FindObjectOfType<SetingsButton>();
        //проверка магазина
        setingsButton.CheckBuyButtonAvailabilityA();
    }

}
