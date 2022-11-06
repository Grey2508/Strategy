using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static float CellSize = 1;

    public Camera MainCamera;
    public Building CurrentBuilding;

    public Transform GroundTransform;

    public static Dictionary<Vector2Int, Building> BuildingDictionary = new Dictionary<Vector2Int, Building>();

    private Plane _plane;
    
    void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);

        BuildingDictionary.Clear();

        Management.InstallCreatedBuilding(this);

        //if (Management.AllBuildings.Count > 0)
        //    foreach (Building building in Management.AllBuildings)
        //    {
        //        int x = Mathf.RoundToInt(building.transform.position.x);
        //        int z = Mathf.RoundToInt(building.transform.position.z);

        //        InstallBuilding(x, z, building);
        //    }
    }

    void Update()
    {
        if (!CurrentBuilding)
            return;

        Management.CurrentSelectionState = SelectionState.Constuction;

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        
        _plane.Raycast(ray, out float distance);

        Vector3 point = ray.GetPoint(distance) / CellSize;

        int x = Mathf.RoundToInt(point.x);
        int z = Mathf.RoundToInt(point.z);

        CurrentBuilding.transform.position = new Vector3(x, 0, z) * CellSize;

        if (CheckAllow(x, z, CurrentBuilding))
        {
            CurrentBuilding.DisplayAcceptablePosition();

            if (Input.GetMouseButtonDown(0))
            {
                InstallBuilding(x, z, CurrentBuilding);

                CurrentBuilding = null;
            }
        }
        else
            CurrentBuilding.DisplayUnacceptablePosition();

        if(Input.GetMouseButtonDown(1))
        {
            CancelBuilding();
        }
    }

    bool CheckAllow(int xPosition, int zPosition, Building building)
    {
        if (Management.IsOnMenu)
            return false;

        if ((xPosition < -GroundTransform.localScale.x / 2)
        || (zPosition < -GroundTransform.localScale.y / 2)
        || ((xPosition + building.XSize) > GroundTransform.localScale.x / 2)
        || ((zPosition + building.ZSize) > GroundTransform.localScale.y / 2))
            return false;

        for (int x = 0; x < building.XSize; x++)
        {
            for (int z = 0; z < building.ZSize; z++)
            {
                Ray ray = new Ray(MainCamera.transform.position, (new Vector3(xPosition + x, 0, zPosition + z) - MainCamera.transform.position));

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (!hit.collider.CompareTag("Ground"))
                    {
                        return false;
                    }
                }

                Vector2Int coordinate = new Vector2Int(xPosition + x, zPosition + z);

                if (BuildingDictionary.ContainsKey(coordinate))
                    return false;
            }
        }

        return true;
    }

    public void InstallBuilding(int xPosition, int zPosition, Building building)
    {
        for (int x = 0; x < building.XSize; x++)
        {
            for (int z = 0; z < building.ZSize; z++)
            {
                Vector2Int coordinate = new Vector2Int(xPosition + x, zPosition + z);
                BuildingDictionary.Add(coordinate, CurrentBuilding);
            }
        }

        building.OnInstall();

        Management.CurrentSelectionState = SelectionState.Other;
    }

    public static void RemoveBuilding(Building building)
    {
        int startX = (int)building.transform.position.x;
        int startZ = (int)building.transform.position.z;
        for (int x = 0; x < building.XSize; x++)
        {
            for (int z = 0; z < building.ZSize; z++)
            {
                Vector2Int coordinate = new Vector2Int(startX + x, startZ + z);
                BuildingDictionary.Remove(coordinate);
            }
        }
    }

    public void CreateBuilding(GameObject buildingPrefab)
    {
        CancelBuilding();

        CurrentBuilding = Instantiate(buildingPrefab).GetComponent<Building>(); ;
    }

    public void CancelBuilding()
    {
        if (CurrentBuilding)
        {
            Resources.Gold += CurrentBuilding.Price;

            Destroy(CurrentBuilding.gameObject);

            CurrentBuilding = null;
        }
    }
}
