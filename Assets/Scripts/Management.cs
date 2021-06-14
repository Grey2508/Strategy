using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectionState
{
    UnitsSelected,
    Frame,
    Constuction,
    Other
}

public class Management : MonoBehaviour
{
    public static List<Enemy> AllEnemies = new List<Enemy>();
    public static List<Unit> AllUnits = new List<Unit>();
    public static List<Building> AllBuildings = new List<Building>();
    public static SelectionState CurrentSelectionState = SelectionState.Other;
    public static bool IsOnMenu = false;

    public Camera MainCamera;
    public SelectableObject Howered;

    public List<SelectableObject> ListOfSelected;

    public Image FrameImage;
    private Vector2 _frameStart;
    private Vector2 _frameEnd;


    void Update()
    {
        RaycastHit hit;

        TryHower(out hit);

        TrySelect();

        ClickOnGround(hit);

        if (Input.GetMouseButtonDown(1))
            UnselectAll();

        FrameSelect();

        if (CurrentSelectionState == SelectionState.Constuction)
            UnselectAll();
    }

    private void TryHower(out RaycastHit hit)
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        if (Physics.Raycast(ray, out hit))
        {
            SelectableObject hitSelectable = hit.collider.GetComponent<SelectableCollider>()?.SelectableObject;

            if (hitSelectable)
            {
                if (Howered)
                    Howered.OnUnhover();

                Howered = hitSelectable;
                Howered.OnHover();
            }
            else
            {
                UnhowerCurrent();
            }
        }
        else
        {
            UnhowerCurrent();
        }
    }

    private void ClickOnGround(RaycastHit hit)
    {
        if (CurrentSelectionState != SelectionState.UnitsSelected)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            if (!hit.collider || !hit.collider.CompareTag("Ground"))
                return;

            int rowNumber = Mathf.CeilToInt(Mathf.Sqrt(ListOfSelected.Count));

            for (int i = 0; i < ListOfSelected.Count; i++)
            {
                int row = i / rowNumber;
                int column = i % rowNumber;

                Vector3 point = hit.point - new Vector3(row, 0, column);

                ListOfSelected[i].WhenClickOnGround(point);
            }
        }
    }

    private void TrySelect()
    {
        if (CurrentSelectionState == SelectionState.Frame
            || CurrentSelectionState == SelectionState.Constuction)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            if (Howered)
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                    UnselectAll();

                CurrentSelectionState = SelectionState.UnitsSelected;

                Select(Howered);
            }
        }
    }

    private void FrameSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _frameStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _frameEnd = Input.mousePosition;

            Vector2 min = Vector2.Min(_frameStart, _frameEnd);
            Vector2 max = Vector2.Max(_frameStart, _frameEnd);
            Vector2 size = max - min;

            if (size.magnitude > 10)
            {
                FrameImage.enabled = true;

                FrameImage.rectTransform.anchoredPosition = min;

                FrameImage.rectTransform.sizeDelta = size;

                Rect rect = new Rect(min, size);

                UnselectAll();

                //Unit[] allUnits = FindObjectsOfType<Unit>();
                foreach (Unit unit in AllUnits)
                {
                    Vector2 screenPosition = MainCamera.WorldToScreenPoint(unit.transform.position);
                    if (rect.Contains(screenPosition))
                    {
                        Select(unit);
                    }
                }

                CurrentSelectionState = SelectionState.Frame;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            FrameImage.enabled = false;

            CurrentSelectionState = (ListOfSelected.Count > 0 ? SelectionState.UnitsSelected : SelectionState.Other);
        }
    }

    private void UnhowerCurrent()
    {
        if (Howered)
        {
            Howered.OnUnhover();
            Howered = null;
        }
    }

    private void Select(SelectableObject selected)
    {
        if (!ListOfSelected.Contains(selected))
        {
            ListOfSelected.Add(selected);
            selected.Select();
        }
    }
    public void UnSelect(SelectableObject selected)
    {
        if (!ListOfSelected.Contains(selected))
        {
            ListOfSelected.Remove(selected);
        }
    }

    private void UnselectAll()
    {
        foreach (SelectableObject item in ListOfSelected)
        {
            if (item)
                item.Unselect();
        }

        ListOfSelected.Clear();
        CurrentSelectionState = SelectionState.Other;
    }
}
