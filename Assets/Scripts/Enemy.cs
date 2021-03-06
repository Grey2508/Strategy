using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    WalkToBuilding,
    WalkToUnit,
    AttackUnit,
    AttackBuilding
}

public class Enemy : MonoBehaviour
{
    public EnemyState CurrentEnemyState;

    public int Health = 5;
    public GameObject HealthBarPrefab;

    public Building TargetBuilding;
    public Unit TargetUnit;

    public float AttackRange = 1;
    public float FollowRange = 7;

    public NavMeshAgent NavMeshAgent;

    public float AttackPeriod = 1;
    public int Damage = 1;

    private float _timer;
    private int _maxHealth;
    private HealthBar _healthBar;

    private void Start()
    {
        Management.AllEnemies.Add(this);

        GameObject healthBarPrefab = Instantiate(HealthBarPrefab);
        _healthBar = healthBarPrefab.GetComponent<HealthBar>();
        _healthBar.Setup(transform);

        _maxHealth = Health;

        SetState(EnemyState.WalkToBuilding);
    }
    void Update()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Idle:
                EnemyIdle();
                break;
            case EnemyState.WalkToBuilding:
                EnemyWalkToBuilding();
                break;
            case EnemyState.AttackBuilding:
                EnemyAttackBuilding();
                break;
            case EnemyState.WalkToUnit:
                EnemyWalkToUnit();
                break;
            case EnemyState.AttackUnit:
                EnemyAttackUnit();
                break;
        }
    }

    private void EnemyIdle()
    {
        FindClosestBuilding();

        if (TargetBuilding)
        {
            SetState(EnemyState.WalkToBuilding);
            return;
        }

        FindClosestUnit();
    }

    private void EnemyWalkToBuilding()
    {
        FindClosestUnit();

        if (!TargetBuilding)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, TargetBuilding.transform.position);
        if (distance < AttackRange)
            SetState(EnemyState.AttackBuilding);
    }

    private void EnemyAttackBuilding()
    {
        if (!TargetBuilding)
        {
            SetState(EnemyState.WalkToBuilding);
            return;
        }

        float distance = Vector3.Distance(transform.position, TargetBuilding.transform.position);

        if (distance > AttackRange)
        {
            SetState(EnemyState.WalkToBuilding);
        }

        _timer += Time.deltaTime;
        if (_timer > AttackPeriod)
        {
            TargetBuilding.TakeDamage(Damage);
            _timer = 0;
        }
    }

    private void EnemyWalkToUnit()
    {
        if (!TargetUnit)
        {
            SetState(EnemyState.WalkToBuilding);
            return;
        }

        NavMeshAgent.SetDestination(TargetUnit.transform.position);
        float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);

        if (distance > FollowRange)
            SetState(EnemyState.WalkToBuilding);
        if (distance < AttackRange)
        {
            SetState(EnemyState.AttackUnit);
        }
    }

    private void EnemyAttackUnit()
    {
        if (!TargetUnit)
        {
            SetState(EnemyState.WalkToBuilding);
            return;
        }

        NavMeshAgent.SetDestination(TargetUnit.transform.position);

        float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);

        if (distance > AttackRange)
        {
            SetState(EnemyState.WalkToUnit);
        }

        _timer += Time.deltaTime;
        if (_timer > AttackPeriod)
        {
            TargetUnit.TakeDamage(Damage);
            _timer = 0;
        }
    }

    public void SetState(EnemyState newState)
    {
        CurrentEnemyState = newState;

        switch (CurrentEnemyState)
        {
            case EnemyState.Idle:
                FindClosestBuilding();

                break;
            case EnemyState.WalkToBuilding:
                {
                    FindClosestBuilding();

                    if (TargetBuilding)
                        NavMeshAgent.SetDestination(TargetBuilding.transform.position);
                    else
                        SetState(EnemyState.Idle);

                    break;
                }
            case EnemyState.AttackBuilding:
                _timer = 0;
                break;
            case EnemyState.WalkToUnit:
                break;
            case EnemyState.AttackUnit:
                _timer = 0;
                break;
        }
    }

    public void FindClosestBuilding()
    {
        //Building[] allBuildings = FindObjectsOfType<Building>();

        float minDistance = Mathf.Infinity;
        Building closestBuilding = null;

        foreach (Building building in Management.AllBuildings)
        {
            if (building.CurrentState == BuildingState.Placed)
                continue;

            float distance = Vector3.Distance(transform.position, building.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestBuilding = building;
            }
        }

        TargetBuilding = closestBuilding;
    }

    public void FindClosestUnit()
    {
        //Unit[] allUnits = FindObjectsOfType<Unit>();

        float minDistance = Mathf.Infinity;
        Unit closestUnit = null;

        foreach (Unit unit in Management.AllUnits)
        {
            float distance = Vector3.Distance(transform.position, unit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestUnit = unit;
            }
        }

        if (minDistance < FollowRange)
        {
            TargetUnit = closestUnit;
            SetState(EnemyState.WalkToUnit);
        }
    }

    public void TakeDamage(int damageValue)
    {
        FindClosestUnit();

        Health -= damageValue;
        _healthBar.SetHealth(Health, _maxHealth);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (_healthBar)
            Destroy(_healthBar.gameObject);

        Management.AllEnemies.Remove(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, AttackRange);

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, FollowRange);
    }
#endif
}
