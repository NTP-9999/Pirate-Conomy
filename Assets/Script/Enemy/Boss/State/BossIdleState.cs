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

        // only chase if too far away
        if (dist > boss.detectionRange)
        {
            boss.stateMachine.ChangeState(new WalkState(boss));
            return;
        }

        // if within detection but outside stoppingDistance, let WalkState handle it
        if (dist > boss.agent.stoppingDistance)
        {
            boss.stateMachine.ChangeState(new WalkState(boss));
            return;
        }

        // otherwise we're in attack range—face & attack
        boss.FacePlayer();
        if (!boss.CanAttack()) return;
        // … pick your attack …
    }

    public void Exit() { }
}
