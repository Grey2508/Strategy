using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{
    private static int _gold;
    public static int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            _goldCountText.text = value.ToString();
        }
    }

    private static Text _goldCountText;
    public int StartGold = 50;

    private void Start()
    {
        _goldCountText = FindObjectOfType<Gold>().GetText();
        Gold = StartGold;
    }
}
