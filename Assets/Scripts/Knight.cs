using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Knight : Unit
{
    public Enemy TargetEnemy;

    public float AttackRange = 1;
    public float FollowRange = 7;

    public float AttackPeriod = 1;
    public int Damage = 1;
    private float _timer;

    public override void Start()
    {
        base.Start();
    
        SetState(UnitState.Idle);
    }
    void Update()
    {
        switch (CurrentUnitState)
        {
            case UnitState.Idle:
                KnightIdle();
                break;
            case UnitState.WalkToPoint:
                KnightWalkToPoint();
                break;
            case UnitState.WalkToEnemy:
                KnightWalkToEnemy();
                break;
            case UnitState.Attack:
                KnightAttack();
                break;
        }
    }

    private void KnightIdle()
    {
        FindClosestEnemy();
    }

    private void KnightWalkToPoint()
    {
        if (FindClosestEnemy())
            return;

        if (Vector3.Distance(transform.position, TargetPoint) >= 0.5f)
        {
            NavMeshAgent.SetDestination(TargetPointer.transform.position);
            SetState(UnitState.WalkToPoint);
        }
        else
            SetState(UnitState.Idle);
    }

    private void KnightWalkToEnemy()
    {
        if (!TargetEnemy)
        {
            SetState(UnitState.WalkToPoint);
            return;
        }

        NavMeshAgent.SetDestination(TargetEnemy.transform.position);
        float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);

        if (distance > FollowRange)
            SetState(UnitState.WalkToPoint);
        if (distance < AttackRange)
        {
            SetState(UnitState.Attack);
        }
    }

    private void KnightAttack()
    {
        if (!TargetEnemy)
        {
            SetState(UnitState.WalkToPoint);
            return;
        }

        NavMeshAgent.SetDestination(TargetEnemy.transform.position);

        float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);

        if (distance > AttackRange)
        {
            SetState(UnitState.WalkToEnemy);
        }

        _timer += Time.deltaTime;
        if (_timer > AttackPeriod)
        {
            TargetEnemy.TakeDamage(Damage);
            _timer = 0;
        }

    }

    public void SetState(UnitState newState)
    {
        CurrentUnitState = newState;

        switch (CurrentUnitState)
        {
            case UnitState.Idle:
                TargetPointer.SetActive(false);
                break;
            case UnitState.WalkToPoint:
                NavMeshAgent.stoppingDistance = 0.2f;
                break;
            case UnitState.WalkToEnemy:
                break;
            case UnitState.Attack:
                NavMeshAgent.stoppingDistance = AttackRange;
                break;
        }
    }

    public bool FindClosestEnemy()
    {
        //Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        //float minDistance = Mathf.Infinity;
        Enemy closestEnemy = Management.GetClosestEnemy(transform.position, out float minDistance);

        //foreach (Enemy enemy in Management.AllEnemies)
        //{
        //    float distance = Vector3.Distance(transform.position, enemy.transform.position);

        //    if (distance < minDistance)
        //    {
        //        minDistance = distance;
        //        closestEnemy = enemy;
        //    }
        //}

        if (minDistance < FollowRange)
        {
            TargetEnemy = closestEnemy;
            SetState(UnitState.WalkToEnemy);

            return true;
        }

        return false;
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