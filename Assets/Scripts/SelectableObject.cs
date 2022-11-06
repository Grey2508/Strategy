using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject SelectionIndicator;
    public int Health = 5;

    public GameObject HealthBarPrefab;
    public float HealthBarHeight = 2;

    internal HealthBar _healthBar;
    internal int _maxHealth;

    private void Start()
    {
        Prepaire();
    }

    protected virtual void Prepaire()
    {
        SelectionIndicator.SetActive(false);

        GameObject healthBarPrefab = Instantiate(HealthBarPrefab);
        _healthBar = healthBarPrefab.GetComponent<HealthBar>();
        _healthBar.Setup(transform, HealthBarHeight);

        _maxHealth = Health;

        _healthBar.gameObject.SetActive(false);
    }

    public virtual void OnHover()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public virtual void OnUnhover()
    {
        transform.localScale = Vector3.one;
    }

    public virtual void Select()
    {
        SelectionIndicator.SetActive(true);
        _healthBar.gameObject.SetActive(true);
    }

    public virtual void Unselect()
    {
        SelectionIndicator.SetActive(false);
        _healthBar.gameObject.SetActive(false);
    }

    public virtual void WhenClickOnGround(Vector3 point)
    {

    }

    private void OnDestroy()
    {
        Destroing();
    }

    protected virtual void Destroing()
    {
        if (_healthBar)
            Destroy(_healthBar.gameObject);
    }
}
