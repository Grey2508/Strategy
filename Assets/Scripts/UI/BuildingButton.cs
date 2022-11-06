using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public BuildingPlacer BuildingPlacer;
    public GameObject BuildingPrefab;

    public Text PriceText;

    private void Start()
    {
        PriceText.text = BuildingPrefab.GetComponent<Building>().Price.ToString();
    }

    public void TryBuy()
    {
        int price = BuildingPrefab.GetComponent<Building>().Price;

        if (Resources.Gold >= price)
        {
            Resources.Gold -= price;

            BuildingPlacer.CreateBuilding(BuildingPrefab);
        }
        else
            NeedMoreGold.Instance.PulseText.StartEffect();
    }
}
