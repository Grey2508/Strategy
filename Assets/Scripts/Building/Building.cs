using UnityEngine;

public enum BuildingState
{
    Placed,
    Ready
}
public class Building : SelectableObject
{
    [Header("Building")]
    public BuildingState CurrentState = BuildingState.Placed;

    public int Price;
    public int XSize = 3;
    public int ZSize = 3;

    public Renderer Renderer;
    private Color _startColor;

    public BoxCollider BuildingCollider;

    protected override void Prepaire()
    {
        base.Prepaire();

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
        BuildingCollider.enabled = true;

        CurrentState = BuildingState.Ready;
    }

    protected override void Destroing()
    {
        base.Destroing();

        BuildingPlacer.RemoveBuilding(this);

        Management.RemoveBuilding(this);
    }
}
