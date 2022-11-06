using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedMoreGold : MonoBehaviour
{
    public static NeedMoreGold Instance;

    public PulseText PulseText;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);
    }
}
