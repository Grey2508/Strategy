using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public enum SelectionState
{
    UnitsSelected,
    Frame,
    Constuction,
    Other
}

public class Management : MonoBehaviour
{
    private static List<Enemy> AllEnemies = new List<Enemy>();
    private static List<Unit> AllUnits = new List<Unit>();
    private static List<Building> AllBuildings = new List<Building>();
    public static SelectionState CurrentSelectionState = SelectionState.Other;
    public static bool IsOnMenu = false;

    public Camera MainCamera;
    public SelectableObject Howered;

    public List<SelectableObject> ListOfSelected;

    public Image FrameImage;
    private Vector2 _frameStart;
    private Vector2 _frameEnd;
    private bool _isFrameActive;

    void Update()
    {
        RaycastHit hit;

        TryHower(out hit);

        TrySelect();

        ClickOnGround(hit);

        //if (Input.GetMouseButtonDown(1))
        //    UnselectAll();

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

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!hit.collider || !hit.collider.CompareTag("Ground"))
                return;

            UnselectAll();
        }

        if (Input.GetMouseButtonUp(1))
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
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            _frameStart = Input.mousePosition;
            _isFrameActive = true;
        }

        if (Input.GetMouseButton(0) && _isFrameActive)
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
            FrameImage.enabled = _isFrameActive = false;

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

    public static void AddEnemy(Enemy newEnemy)
    {
        AllEnemies.Add(newEnemy);
    }
    public static void RemoveEnemy(Enemy enemy)
    {
        AllEnemies.Remove(enemy);
    }

    public static void AddUnit(Unit newUnit)
    {
        AllUnits.Add(newUnit);
    }
    public static void RemoveUnit(Unit unit)
    {
        AllUnits.Remove(unit);
    }

    public static void AddBuilding(Building newBuilding)
    {
        AllBuildings.Add(newBuilding);
    }
    public static void RemoveBuilding(Building building)
    {
        AllBuildings.Remove(building);

        if (AllBuildings.Count == 0)
            GameOver();
    }

    private static void GameOver()
    {
        Debug.Log("gameOver");
    }

    public static T GetClosestBuilding<T>(Vector3 position, out float Distance) where T : Building
    {
        float minDistance = float.MaxValue;
        T closestBuilding = null;

        foreach (Building building in AllBuildings)
        {
            if (!(building is T))
                continue;

            if (building.CurrentState == BuildingState.Placed)
                continue;

            float distance = Vector3.Distance(position, building.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestBuilding = (T)building;
            }
        }

        Distance = minDistance;
        return closestBuilding;
    }

    public static T GetClosestUnit<T>(Vector3 position, out float Distance) where T : Unit
    {
        float minDistance = float.MaxValue;
        T closestUnit = null;

        foreach (Unit unit in AllUnits)
        {
            if (!(unit is T))
                continue;

            float distance = Vector3.Distance(position, unit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestUnit = (T)unit;
            }
        }

        Distance = minDistance;
        return closestUnit;
    }

    public static Enemy GetClosestEnemy(Vector3 position, out float Distance)
    {
        float minDistance = Distance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (Enemy enemy in AllEnemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        Distance = minDistance;
        return closestEnemy;
    }

    public static void InstallCreatedBuilding(BuildingPlacer buildingPlacer)
    {
        if (AllBuildings.Count == 0)
            return;

        foreach (Building building in AllBuildings)
        {
            int x = Mathf.RoundToInt(building.transform.position.x);
            int z = Mathf.RoundToInt(building.transform.position.z);

            buildingPlacer.InstallBuilding(x, z, building);
        }
    }
}
