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

        float dist = Vector3.Distance(boss.transform.position, boss.player.position);
        // once you're close enough (agent.stoppingDistance), go back to idle
        if (dist <= boss.agent.stoppingDistance)
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
