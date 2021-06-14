using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SelectionState _prevSelectionState;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Management.IsOnMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Management.IsOnMenu = false;
    }
}
