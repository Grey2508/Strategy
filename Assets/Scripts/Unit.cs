using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    WalkToPoint,
    WalkToEnemy,
    Attack,
    Mining,
    Escape
}

public class Unit : SelectableObject
{
    public NavMeshAgent NavMeshAgent;
    public int Price = 5;

    public GameObject TargetPointer;
    public Vector3 TargetPoint;

    public UnitState CurrentUnitState;

    public override void Start()
    {
        base.Start();
        TargetPointer.transform.parent = null;
        TargetPoint = transform.position;

        TargetPointer.SetActive(false);

        Management.AddUnit(this);
    }
    public override void WhenClickOnGround(Vector3 point)
    {
        base.WhenClickOnGround(point);

        NavMeshAgent.SetDestination(point);
        CurrentUnitState = UnitState.WalkToPoint;

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

    public override void OnDestroy()
    {
        base.OnDestroy();

        FindObjectOfType<Management>()?.UnSelect(this);

        if (TargetPointer)
            Destroy(TargetPointer);

        Management.RemoveUnit(this);
    }

    public override void Select()
    {
        base.Select();

        if (CurrentUnitState != UnitState.Idle)
            TargetPointer.SetActive(true);
    }

    public override void Unselect()
    {
        base.Unselect();

        TargetPointer.SetActive(false);
    }
}
