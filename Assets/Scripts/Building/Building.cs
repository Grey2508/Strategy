using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
    Placed,
    Ready
}
public class Building : SelectableObject
{
    public BuildingState CurrentState = BuildingState.Placed;

    //public int Health = 10;

    public int Price;
    public int XSize = 3;
    public int ZSize = 3;

    public Renderer Renderer;
    private Color _startColor;

    public GameObject MenuObject;

    public Transform Spawn;

    public BoxCollider BuildingCollider;

    //public GameObject HealthBarPrefab;
    //public float HeightHealthBar = 2;

    //private int _maxHealth = 5;
    //private HealthBar _healthBar;

    public override void Start()
    {
        base.Start();
        MenuObject.SetActive(false);

        Management.AddBuilding(this);
    }

    private void Awake()
    {
        _startColor = Renderer.material.color;
    }

    private void OnDrawGizmos()
    {
        float cellSize = BuildingPlacer.CellSize;

        for (int x = 0; x < XSize; x++)
            for (int z = 0; z < ZSize; z++)
                Gizmos.DrawWireCube(transform.position + new Vector3(x, 0, z) * cellSize, new Vector3(1, 0, 1) * cellSize);
    }

    public void DisplayUnacceptablePosition()
    {
        Renderer.material.color = Color.red;
    }

    public void DisplayAcceptablePosition()
    {
        Renderer.material.color = _startColor;
    }

    public override void Select()
    {
        base.Select();

        MenuObject.SetActive(true);
    }

    public override void Unselect()
    {
        base.Unselect();

        MenuObject.SetActive(false);
    }

    public virtual void CreateUnit(GameObject unitPrefab)
    {
        GameObject newUnit = Instantiate(unitPrefab, Spawn.position, Quaternion.identity);

        Vector3 position = Spawn.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));

        newUnit.GetComponent<Unit>().WhenClickOnGround(position);
    }

    public void TakeDamage(int damageValue)
    {
        Health -= damageValue;
        _healthBar.SetHealth(Health, _maxHealth);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnInstall()
    {
        //GameObject healthBarPrefab = Instantiate(HealthBarPrefab);
        //_healthBar = healthBarPrefab.GetComponent<HealthBar>();
        //_healthBar.Setup(SelectionIndicator.transform, HeightHealthBar);

        //_maxHealth = Health;

        BuildingCollider.enabled = true;

        CurrentState = BuildingState.Ready;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        BuildingPlacer.RemoveBuilding(this);

        Management.RemoveBuilding(this);
    }
}
