using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateUnitButton : MonoBehaviour
{
    public GameObject UnitPrefab;

    public Text PriceText;

    public Building Building;

    private void Start()
    {
        PriceText.text = UnitPrefab.GetComponent<Unit>().Price.ToString();
    }

    public void TryBuy()
    {
        int price = UnitPrefab.GetComponent<Unit>().Price;

        if (Resources.Gold >= price)
        {
            Resources.Gold -= price;

            Building.CreateUnit(UnitPrefab);
        }
        else
            Debug.Log("You need more money!!!");
    }
}
