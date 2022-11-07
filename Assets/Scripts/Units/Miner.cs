using UnityEditor;
using UnityEngine;

public enum MinerState
{
    Idle,
    WalkToPoint,
    Mining,
    Escape
}

public class Miner : Unit
{
    [Header("Miner")]
    public MinerState CurrentState;

    public float FearRange = 6;
    public float MiningRange = 1;
    public float SeeMineRange = 8;

    private Mine _targetMine;

    private Enemy _scaredEnemy;
    private Knight _knight;

    protected override void Prepaire()
    {
        base.Prepaire();

        SetState(MinerState.Idle);
    }

    void Update()
    {
        if (CurrentState != MinerState.Escape)
            FindClosestEnemy();

        switch (CurrentState)
        {
            case MinerState.Idle:
                MinerIdle();
                break;
            case MinerState.WalkToPoint:
                MinerWalkToPoint();
                break;
            case MinerState.Mining:
                MinerMining();
                break;
            case MinerState.Escape:
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
                SetState(MinerState.WalkToPoint);
            }
            else
                SetState(MinerState.Idle);
        }
        else
        {
            NavMeshAgent.SetDestination(_targetMine.transform.position);
            float distance = Vector3.Distance(transform.position, _targetMine.transform.position);

            if (distance < MiningRange)
            {
                SetState(MinerState.Mining);
            }
        }
    }

    private void MinerMining()
    {
    }

    private void MinerEscape()
    {
        if (!FindClosestEnemy())
            SetState(MinerState.WalkToPoint);

        if (FindClosestKnight())
        {
            NavMeshAgent.SetDestination(_knight.transform.position);
        }
        else
        {
            Vector3 toEnemy = _scaredEnemy.transform.position - transform.position;
            Vector3 fromEnemy = transform.position - toEnemy;

            NavMeshAgent.SetDestination(fromEnemy);
        }

    }

    public void SetState(MinerState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case MinerState.Idle:
                TargetPointer.SetActive(false);
                break;
            case MinerState.WalkToPoint:
                break;
            case MinerState.Mining:
                {
                    TargetPointer.SetActive(false);
                    _targetMine.IncCountMiners();

                    break;
                }
            case MinerState.Escape:
                {
                    TargetPointer.SetActive(false);
                    LoseMine();

                    break;
                }
        }
    }

    protected override void SetState(int newState)
    {
        SetState((MinerState)newState);
    }

    public bool FindClosestEnemy()
    {
        Enemy closestEnemy = Management.GetClosestEnemy(transform.position, out float minDistance);

        if (minDistance < FearRange)
        {
            _scaredEnemy = closestEnemy;
            
            LoseMine();

            SetState(MinerState.Escape);

            return true;
        }

        return false;
    }

    public bool FindClosestKnight()
    {
        Knight closestKnight = Management.GetClosestUnit<Knight>(transform.position, out float minDistance);

        if (closestKnight)
        {
            _knight = closestKnight;

            return true;
        }

        return false;
    }

    public void FindClosestMine()
    {
        Mine closestMine = Management.GetClosestBuilding<Mine>(transform.position, out float minDistance);

        if (minDistance < SeeMineRange)
        {
            _targetMine = closestMine;

            SetState(MinerState.WalkToPoint);
        }
    }

    public override void WhenClickOnGround(Vector3 point)
    {
        LoseMine();

        base.WhenClickOnGround(point);
        
        SetState(MinerState.WalkToPoint);
    }

    private void LoseMine()
    {
        if (CurrentState == MinerState.Mining)
            _targetMine?.DecCountMiners();

        _targetMine = null;
    }

    protected override void Destroing()
    {
        base.Destroing();

        LoseMine();
    }

    public override bool IsFree()
    {
        return CurrentState == MinerState.Mining;
    }

    protected override bool IsIdle()
    {
        return CurrentState == MinerState.Idle;
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
