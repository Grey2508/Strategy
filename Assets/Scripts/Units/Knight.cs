using UnityEditor;
using UnityEngine;

public enum KnightState
{
    Idle,
    WalkToPoint,
    WalkToEnemy,
    Attack,
}

public class Knight : Unit
{
    [Header("Knight")]
    public KnightState CurrentState;

    public Enemy TargetEnemy;

    public float AttackRange = 1;
    public float FollowRange = 7;

    public float AttackPeriod = 1;
    public int Damage = 1;
    private float _timer;

    protected override void Prepaire()
    {
        base.Prepaire();
    
        SetState(KnightState.Idle);
    }

    void Update()
    {
        switch (CurrentState)
        {
            case KnightState.Idle:
                KnightIdle();
                break;
            case KnightState.WalkToPoint:
                KnightWalkToPoint();
                break;
            case KnightState.WalkToEnemy:
                KnightWalkToEnemy();
                break;
            case KnightState.Attack:
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
            SetState(KnightState.WalkToPoint);
        }
        else
            SetState(KnightState.Idle);
    }

    private void KnightWalkToEnemy()
    {
        if (!TargetEnemy)
        {
            SetState(KnightState.WalkToPoint);
            return;
        }

        NavMeshAgent.SetDestination(TargetEnemy.transform.position);
        float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);

        if (distance > FollowRange)
            SetState(KnightState.WalkToPoint);
        if (distance < AttackRange)
        {
            SetState(KnightState.Attack);
        }
    }

    private void KnightAttack()
    {
        if (!TargetEnemy)
        {
            SetState(KnightState.WalkToPoint);
            return;
        }

        NavMeshAgent.SetDestination(TargetEnemy.transform.position);

        float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);

        if (distance > AttackRange)
        {
            SetState(KnightState.WalkToEnemy);
        }

        _timer += Time.deltaTime;
        if (_timer > AttackPeriod)
        {
            TargetEnemy.TakeDamage(Damage);
            _timer = 0;
        }

    }

    public void SetState(KnightState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case KnightState.Idle:
                TargetPointer.SetActive(false);
                break;
            case KnightState.WalkToPoint:
                NavMeshAgent.stoppingDistance = 0.2f;
                break;
            case KnightState.WalkToEnemy:
                break;
            case KnightState.Attack:
                NavMeshAgent.stoppingDistance = AttackRange;
                break;
        }
    }

    protected override void SetState(int newState)
    {
        SetState((KnightState)newState);
    }

    public bool FindClosestEnemy()
    {
        Enemy closestEnemy = Management.GetClosestEnemy(transform.position, out float minDistance);

        if (minDistance < FollowRange)
        {
            TargetEnemy = closestEnemy;
            SetState(KnightState.WalkToEnemy);

            return true;
        }

        return false;
    }

    protected override bool IsIdle()
    {
        return CurrentState == KnightState.Idle;
    }

    public override bool IsFree()
    {
        return CurrentState != KnightState.WalkToEnemy && CurrentState != KnightState.Attack;
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