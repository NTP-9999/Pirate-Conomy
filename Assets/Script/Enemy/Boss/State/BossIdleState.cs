using UnityEngine;
using UnityEngine.AI;

public class BossIdleState : BossIState
{
    private readonly IceDragonBossAI boss;
    private readonly float attackRange;

    public BossIdleState(IceDragonBossAI boss)
    {
        this.boss       = boss;
        // use the agent's stoppingDistance as your attack threshold
        attackRange     = boss.agent.stoppingDistance;
    }

    public void Enter()
    {
        // clear any leftover triggers
        boss.animator.ResetTrigger("Roar");
        boss.animator.ResetTrigger("TailSwipe");
        boss.animator.ResetTrigger("Scratch");

        // stop movement and play idle anim
        boss.agent.isStopped        = true;
        boss.animator.SetBool("IsMoving", false);
        boss.animator.Play("Idle");
    }

    public void Tick()
    {
        float dist = Vector3.Distance(boss.transform.position, boss.player.position);

        // 1) If outside detectionRange, do nothing (or you could add a PatrolState here)
        if (dist > boss.detectionRange)
            return;

        // 2) If within detectionRange but still outside attackRange → chase
        if (dist > attackRange)
        {
            boss.stateMachine.ChangeState(new WalkState(boss));
            return;
        }

        // 3) Now we're within attackRange → face & attack
        boss.FacePlayer();
        if (!boss.CanAttack()) return;

        int choice = Random.Range(0, 3);
        switch (choice)
        {
            case 0: boss.stateMachine.ChangeState(new RoarState(boss));       break;
            case 1: boss.stateMachine.ChangeState(new TailSwipeState(boss));  break;
            case 2: boss.stateMachine.ChangeState(new ScratchState(boss));    break;
        }
    }

    public void Exit() { }
}
