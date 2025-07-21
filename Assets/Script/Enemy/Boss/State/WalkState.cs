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
        boss.agent.isStopped = false;
        boss.animator.SetBool("IsMoving", true);
        // ใส่ path นึงก่อนเลย เพื่อให้ remainingDistance ทันที (แต่เราจะไม่ใช้)
        boss.agent.SetDestination(boss.player.position);
    }

    public void Tick()
    {
        Vector3 offset = Vector3.zero;
        if (Time.frameCount % 120 == 0) {
            float angle = Random.Range(-45f, +45f);
            offset = Quaternion.Euler(0, angle, 0) * (boss.player.position - boss.transform.position).normalized;
        }
        Vector3 target = boss.player.position + offset * 2f;
        boss.agent.SetDestination(target);
            // 1) คำนวณระยะตรง ๆ
        float dist = Vector3.Distance(boss.transform.position, boss.player.position);

        // 2) ถ้าออกนอก detectionRange → หยุดเดิน แล้วกลับ Idle
        if (dist > boss.detectionRange)
        {
            boss.agent.ResetPath();
            boss.stateMachine.ChangeState(new BossIdleState(boss));
            return;
        }

        // 3) ถ้าเข้าใกล้จน <= stoppingDistance → หยุด แล้วกลับ Idle
        if (dist <= boss.agent.stoppingDistance)
        {
            boss.agent.ResetPath();
            boss.stateMachine.ChangeState(new BossIdleState(boss));
            return;
        }

        // 4) ยังคงอยู่ในโซนที่ต้องไล่ → update destination
        boss.agent.SetDestination(boss.player.position);
    }

    public void Exit()
    {
        boss.agent.isStopped = true;
        boss.animator.SetBool("IsMoving", false);
    }
}
