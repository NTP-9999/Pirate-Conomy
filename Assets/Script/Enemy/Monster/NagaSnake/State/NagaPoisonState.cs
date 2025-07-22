using UnityEngine;

public class NagaPoisonState : NagaIState
{
    private SnakeNagaAI ctx;
    private float         enterTime;
    private bool          hasSpawned;

    public NagaPoisonState(SnakeNagaAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        // หยุดทุกอย่างและเล่นแอนิเมชันปล่อยพิษ
        ctx.Agent.isStopped = true;
        ctx.Animator.SetTrigger("Poison");  // ต้องมี Trigger “Poison” ใน Animator
        enterTime   = Time.time;
        hasSpawned  = false;
    }

    public void Execute()
    {
        float elapsed = Time.time - enterTime;

        // หลังดีเลย์ เลิกปล่อย VFX แค่ครั้งเดียว
        if (!hasSpawned && elapsed >= ctx.poisonSpawnDelay)
        {
            ctx.SpawnPoisonVFX();
            hasSpawned = true;
        }

        // พอหมดระยะเวลาของ State ให้ลบ VFX แล้วกลับ State เหมาะสม
        if (elapsed >= ctx.poisonStateDuration)
        {
            ctx.DestroyPoisonVFX();

            // กลับไป Chase/Attack/Patrol ตามระยะผู้เล่น
            if (ctx.IsPlayerInAttackRange())
                ctx.StateMachine.ChangeState(ctx.attackState);
            else if (ctx.IsPlayerInChaseRange())
                ctx.StateMachine.ChangeState(ctx.chaseState);
            else
                ctx.StateMachine.ChangeState(ctx.patrolState);
        }
    }

    public void Exit()
    {
        // ถ้ามีอะไรต้องเคลียร์เพิ่มเติมก็ทำที่นี่
    }
}
