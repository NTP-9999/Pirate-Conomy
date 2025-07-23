// CrabHideState.cs
using UnityEngine;

public class CrabHideState : CrabIState
{
    private readonly CrabAI ctx;

    public CrabHideState(CrabAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        // หยุดทุกอย่าง แล้วสั่งเล่นแอนิเมชัน Hide
        ctx.Agent.isStopped = true;
        ctx.Animator.SetTrigger("Hide");  // ชื่อ Trigger ตามใน Animator ของคุณ
    }

    public void Execute()
    {
        // รอจนผู้เล่นเข้าระยะ ก็ไป ChaseState ทันที
        if (ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit()
    {
        // nothing
    }
}
