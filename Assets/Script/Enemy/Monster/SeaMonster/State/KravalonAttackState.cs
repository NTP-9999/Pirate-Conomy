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
        ctx.animator.SetTrigger("Attack");
        ctx.SetAttackCooldown();
    }

    public void Execute()
    {
        // ถ้ายังอยู่ในระยะโจมตี → กลับไป Idle ตีอีก
        if (ctx.IsShipInAttackRange())
            ctx.StateMachine.ChangeState(ctx.attackIdleState);
        else
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit() { }
}
