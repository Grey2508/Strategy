using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Miner : Unit
{
    public float FearRange = 6;
    public float MiningRange = 1;
    public float SeeMineRange = 8;

    private Mine _targetMine;

    private Enemy _scaredEnemy;
    private Knight _knight;

    public override void Start()
    {
        base.Start();

        SetState(UnitState.Idle);
    }
    void Update()
    {
        if (CurrentUnitState != UnitState.Escape)
            FindClosestEnemy();

        switch (CurrentUnitState)
        {
            case UnitState.Idle:
                MinerIdle();
                    break;
            case UnitState.WalkToPoint:
                MinerWalkToPoint();
                    break;
            case UnitState.Mining:
                MinerMining();
                    break;
            case UnitState.Escape:
                MinerEscape();
                break;
        }
    }

    private void MinerIdle()
    {
        FindClosestMine();
    }

    private void MinerWalkToPoint()
    {
        if (!_targetMine)
        {
            if (Vector3.Distance(transform.position, TargetPoint) >= 0.5f)
            {
                NavMeshAgent.SetDestination(TargetPointer.transform.position);
                SetState(UnitState.WalkToPoint);
            }
            else
                SetState(UnitState.Idle);
        }
        else
        {
            NavMeshAgent.SetDestination(_targetMine.transform.position);
            float distance = Vector3.Distance(transform.position, _targetMine.transform.position);

            if (distance < MiningRange)
            {
                //_targetMine.ToggleIndicator(true);
                SetState(UnitState.Mining);
            }
        }
    }

    private void MinerMining()
    {
    }

    private void MinerEscape()
    {
        if (!FindClosestEnemy())
            SetState(UnitState.WalkToPoint);

        if (FindClosestKnight())
        {
            if (_knight.CurrentUnitState != UnitState.WalkToEnemy && _knight.CurrentUnitState != UnitState.Attack)
                NavMeshAgent.SetDestination(_knight.transform.position);
        }
        else
        {
            Vector3 toEnemy = _scaredEnemy.transform.position - transform.position;
            Vector3 fromEnemy = transform.position - toEnemy;

            NavMeshAgent.SetDestination(fromEnemy);
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
                break;
            case UnitState.Mining:
                {
                    TargetPointer.SetActive(false);
                    _targetMine.IncCountMiners();

                    break;
                }
            case UnitState.Escape:
                {
                    TargetPointer.SetActive(false);
                    LoseMine();

                    break;
                }
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

        if (minDistance < FearRange)
        {
            _scaredEnemy = closestEnemy;
            SetState(UnitState.Escape);

            return true;
        }

        return false;
    }
    public bool FindClosestKnight()
    {
        //float minDistance = Mathf.Infinity;
        Knight closestKnight = Management.GetClosestUnit<Knight>(transform.position, out float minDistance);

        //foreach (Unit knight in Management.AllUnits)
        //{
        //    if (!(knight is Knight))
        //        continue;

        //    float distance = Vector3.Distance(transform.position, knight.transform.position);

        //    if (distance < minDistance)
        //    {
        //        minDistance = distance;
        //        closestKnight = (Knight)knight;
        //    }
        //}

        if (closestKnight)
        {
            _knight = closestKnight;

            return true;
        }

        return false;
    }

    public void FindClosestMine()
    {
        //Mine[] allMines = FindObjectsOfType<Mine>();

        //float minDistance = Mathf.Infinity;
        Mine closestMine = Management.GetClosestBuilding<Mine>(transform.position, out float minDistance);

        //foreach (Building mine in Management.AllBuildings)
        //{
        //    if (!(mine is Mine))
        //        continue;

        //    if (mine.CurrentState == BuildingState.Placed)
        //        continue;

        //    float distance = Vector3.Distance(transform.position, mine.transform.position);

        //    if (distance < minDistance)
        //    {
        //        minDistance = distance;
        //        closestMine = (Mine)mine;
        //    }
        //}

        if (minDistance < SeeMineRange)
        {
            _targetMine = closestMine;

            SetState(UnitState.WalkToPoint);
        }
    }

    public override void WhenClickOnGround(Vector3 point)
    {
        base.WhenClickOnGround(point);

        LoseMine();
    }

    private void LoseMine()
    {
        _targetMine?.DecCountMiners();

        _targetMine = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, FearRange);

        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, MiningRange);
    }
#endif
}
