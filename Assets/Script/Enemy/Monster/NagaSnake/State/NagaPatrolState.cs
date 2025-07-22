// PatrolState.cs
using UnityEngine;
using UnityEngine.AI;

public class NagaPatrolState : NagaIState {
    SnakeNagaAI ctx;
    NavMeshAgent agent;
    float radius, pointThreshold;
    Vector3 patrolPoint;
    bool pointSet;

    public NagaPatrolState(SnakeNagaAI context) {
        ctx = context;
        agent = ctx.Agent;
        radius = ctx.patrolRadius;
        pointThreshold = ctx.patrolPointReachThreshold;
    }

    public void Enter() {
        pointSet = false;
        agent.isStopped = false;
    }

    public void Execute() {
        if (!pointSet) SearchPoint();
        else {
            agent.SetDestination(patrolPoint);
            if (Vector3.Distance(ctx.Position, patrolPoint) < pointThreshold)
                pointSet = false;
        }

        if (ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit() { }
    
    void SearchPoint() {
        Vector3 rnd = Random.insideUnitSphere * radius + ctx.Position;
        if (NavMesh.SamplePosition(rnd, out var hit, radius, NavMesh.AllAreas)) {
            patrolPoint = hit.position;
            pointSet = true;
        }
    }
}
