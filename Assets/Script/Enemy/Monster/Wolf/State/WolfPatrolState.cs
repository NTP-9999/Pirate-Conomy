// WolfPatrolState.cs
using UnityEngine;
using UnityEngine.AI;

public class WolfPatrolState : WolfIState
{
    private WolfAI ctx;
    private NavMeshAgent agent;
    private float radius, pointThreshold;
    private Vector3 patrolPoint;
    private bool pointSet;

    public WolfPatrolState(WolfAI context)
    {
        ctx            = context;
        agent          = ctx.Agent;
        radius         = ctx.patrolRadius;
        pointThreshold = ctx.patrolPointReachThreshold;
    }

    public void Enter()
    {
        pointSet = false;
        agent.isStopped = false;
        ctx.Animator.SetBool("isMoving", true);
    }

    public void Execute()
    {
        if (!pointSet)
            SearchPoint();
        else
        {
            agent.SetDestination(patrolPoint);
            if (Vector3.Distance(ctx.Position, patrolPoint) < pointThreshold)
                pointSet = false;
        }

        if (ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit()
    {
        ctx.Animator.SetBool("isMoving", false);
     }

    private void SearchPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere * radius + ctx.patrolCenter;
        randomDir.y = ctx.patrolCenter.y;

        if (NavMesh.SamplePosition(randomDir, out var hit, radius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            pointSet = true;
        }
    }
}
