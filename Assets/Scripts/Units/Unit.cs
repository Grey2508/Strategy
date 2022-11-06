using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    WalkToPoint
}

public class Unit : SelectableObject
{
    [Header("Unit")]
    public NavMeshAgent NavMeshAgent;
    public int Price = 5;

    public GameObject TargetPointer;
    public Vector3 TargetPoint;

    protected override void Prepaire()
    {
        base.Prepaire();
        TargetPointer.transform.parent = null;
        TargetPoint = transform.position;

        TargetPointer.SetActive(false);

        Management.AddUnit(this);
    }

    public override void WhenClickOnGround(Vector3 point)
    {
        base.WhenClickOnGround(point);

        NavMeshAgent.SetDestination(point);
        SetState((int)UnitState.WalkToPoint);

        TargetPoint = point;

        TargetPointer.transform.position = new Vector3(point.x, 0.02f, point.z);
        TargetPointer.SetActive(true);
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

    protected override void Destroing()
    {
        base.Destroing();

        FindObjectOfType<Management>()?.UnSelect(this);

        if (TargetPointer)
            Destroy(TargetPointer);

        Management.RemoveUnit(this);
    }

    public override void Select()
    {
        base.Select();

        if(IsIdle())
            TargetPointer.SetActive(true);
    }

    public override void Unselect()
    {
        base.Unselect();

        TargetPointer.SetActive(false);
    }
    
    public virtual bool IsFree()
    {
        return false;
    }

    protected virtual bool IsIdle()
    {
        return false;
    }

    protected virtual void SetState (int newState)
    {

    }
}
