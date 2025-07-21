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

        if (!boss.CanAttack()) return;

        if (dist <= 22f) {
            // ระยะใกล้มาก: ใช้ Scratch (กรงเล็บเขี่ย)
            boss.stateMachine.ChangeState(new ScratchState(boss));
        }
        else if (dist <= 35f) {
            // ระยะใกล้ปานกลาง: ใช้ TailSwipe
            boss.stateMachine.ChangeState(new TailSwipeState(boss));
        }
        else if (dist <= 40f) {
            // ระยะกลาง: ใช้ Roar (ดึง aggro, แต่ไม่มีดาเมจ)
            boss.stateMachine.ChangeState(new RoarState(boss));
        }
        else {
            // ระยะไกล: เดินเข้าไปหรือเป่าลมแข็ง (IceBreath) ถ้ามี
            boss.stateMachine.ChangeState(new WalkState(boss));
        }

        // … เลือกท่าโจมตี แล้ว ChangeState ไปเลย …
    }


    public void Exit() { }
}
