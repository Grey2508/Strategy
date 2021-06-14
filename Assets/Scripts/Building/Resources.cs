using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{
    private static int _gold = 50;
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

    private void Start()
    {
        _goldCountText = FindObjectOfType<Gold>().gameObject.GetComponent<Text>();
        _goldCountText.text = Gold.ToString();
    }
}
