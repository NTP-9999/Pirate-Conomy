using UnityEngine;
using UnityEngine.AI;

public class NagaPatrolState : NagaIState {
    SnakeNagaAI ctx;
    NavMeshAgent agent;
    float radius, pointThreshold;
    Vector3 patrolPoint;
    bool pointSet;

    public NagaPatrolState(SnakeNagaAI context) {
        ctx            = context;
        agent          = ctx.Agent;
        radius         = ctx.patrolRadius;
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
        {
            ctx.StateMachine.ChangeState(ctx.chaseState);
            NagaAudioManager.Instance.PlayBGM(NagaAudioManager.Instance.bgmClip, 1f);
        }
            
    }

    public void Exit() { }

    private void SearchPoint()
    {
        // สุ่มตำแหน่งรอบ ๆ patrolCenter (คงที่)
        Vector3 randomDir = Random.insideUnitSphere * radius + ctx.patrolCenter;
        randomDir.y = ctx.patrolCenter.y;

        if (NavMesh.SamplePosition(randomDir, out var hit, radius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            pointSet = true;
        }
    }
}
