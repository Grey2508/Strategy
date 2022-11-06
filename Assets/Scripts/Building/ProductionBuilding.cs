using UnityEngine;

public class ProductionBuilding : Building
{
    [Header("ProductionBuilding")]
    public GameObject MenuObject;
    public Transform Spawn;

    protected override void Prepaire()
    {
        base.Prepaire();

        MenuObject.SetActive(false);
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
}
