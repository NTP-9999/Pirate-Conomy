using UnityEngine;

public class NagaHurtState : NagaIState
{
    private SnakeNagaAI ctx;
    private float startTime;

    public NagaHurtState(SnakeNagaAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        // หยุดเดิน/ไล่
        ctx.Agent.isStopped = true;
        // Trigger แอนิเมชันโดนดาเมจ (ต้องมี trigger "Hurt" ใน Animator)
        ctx.Animator.SetTrigger("Hurt");
        startTime = Time.time;
    }

    public void Execute()
    {
        // รอจนกว่าจะครบ hurtDuration แล้วโยกกลับ state ตามสถานการณ์
        if (Time.time >= startTime + ctx.hurtDuration)
        {
            // ถ้าตอนนี้เข้า Attack range ให้ไปตีต่อ
            if (ctx.IsPlayerInAttackRange())
                ctx.StateMachine.ChangeState(ctx.attackState);

            // ถ้ายังไม่ถึง Attack แต่ถึง Chase range ให้ไล่
            else if (ctx.IsPlayerInChaseRange())
                ctx.StateMachine.ChangeState(ctx.chaseState);

            // มิฉะนั้น กลับไป Patrol
            else
                ctx.StateMachine.ChangeState(ctx.patrolState);
        }
    }

    public void Exit()
    {
        // (ถ้าอยาก reset Animator trigger อื่น ๆ ก็ทำที่นี่)
    }
}
