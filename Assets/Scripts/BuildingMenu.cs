using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Management.IsOnMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Management.IsOnMenu = false;
    }
}
