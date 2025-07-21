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
        float dist = Vector3.Distance(
            boss.transform.position,
            boss.player.position
        );

        // ถ้าไกลเกิน detectionRange → ไม่ทำอะไร
        if (dist > boss.detectionRange)
            return;

        // ถ้ายังไกลกว่า stoppingDistance → เปลี่ยนไป WalkState
        if (dist > boss.agent.stoppingDistance)
        {
            boss.stateMachine.ChangeState(new WalkState(boss));
            return;
        }

        // ถ้าเข้าใกล้พอแล้ว (<= stoppingDistance) → โจมตี
        boss.FacePlayer();
        if (!boss.CanAttack()) return;

        // … เลือกท่าโจมตี แล้ว ChangeState ไปเลย …
    }


    public void Exit() { }
}
