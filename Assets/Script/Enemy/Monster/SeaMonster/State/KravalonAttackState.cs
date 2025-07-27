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
        // หันหน้าไปหาเป้าหมายแบบล็อกทิศ (ถ้าอยากให้มอง)
        Vector3 lookDir = ctx.shipTarget.position - ctx.transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            ctx.transform.rotation = Quaternion.Slerp(ctx.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // ถ้าอยู่ในระยะโจมตี → ตีซ้ำ (กลับไป idle ก่อนตีใหม่)
        if (ctx.IsShipInAttackRange())
        {
            ctx.StateMachine.ChangeState(ctx.attackIdleState);
        }
        else
        {
            ctx.StateMachine.ChangeState(ctx.chaseState);
        }
    }

    public void Exit() { }
}
