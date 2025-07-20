using UnityEngine;
using UnityEngine.AI;

public class WalkState : BossIState
{
    private readonly IceDragonBossAI boss;

    public WalkState(IceDragonBossAI boss)
    {
        this.boss = boss;
    }

    public void Enter()
    {
        boss.agent.isStopped        = false;
        boss.animator.SetBool("IsMoving", true);
    }

    public void Tick()
    {
        // continually set the NavMeshAgent destination
        boss.agent.SetDestination(boss.player.position);

        if (!boss.agent.pathPending && boss.agent.remainingDistance <= boss.agent.stoppingDistance)
        {
            boss.stateMachine.ChangeState(new BossIdleState(boss));
        }
    }

    public void Exit()
    {
        boss.agent.isStopped        = true;
        boss.animator.SetBool("IsMoving", false);
    }
}
