using UnityEngine;
public class KravalonAttackState : IKravalonState
{
    private KravalonAI ctx;

    public KravalonAttackState(KravalonAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        // ยิงครั้งแรกตอนเข้ารัฐนี้
        ctx.animator.SetTrigger("Attack");
        ctx.SetAttackCooldown();    // บันทึกเวลายิง
    }

    public void Execute()
    {
        if (ctx.shipTarget == null) return;

        float distance = Vector3.Distance(ctx.transform.position, ctx.shipTarget.position);
        if (distance > ctx.attackRange)
        {
            // ถ้าไกลเกินระยะ ให้กลับไปวิ่งตาม
            ctx.StateMachine.ChangeState(ctx.chaseState);
            return;
        }

        // หมุนหาทิศเรือ (ล็อค Y) เหมือนกัน
        

        // พอ cooldown ผ่าน → ยิงใหม่
        if (ctx.IsAttackCooldownReady())
        {
            ctx.animator.SetTrigger("Attack");
            ctx.SetAttackCooldown();
        }
    }

    public void Exit() { }
}

