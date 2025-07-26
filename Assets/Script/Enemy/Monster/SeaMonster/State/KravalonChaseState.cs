using UnityEngine;

public class KravalonChaseState : IKravalonState
{
    private KravalonAI ctx;

    public KravalonChaseState(KravalonAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        ctx.animator.SetBool("IsChasing", true);
    }

    public void Execute()
    {
        if (ctx.shipTarget == null) return;

        // ขยับเข้าหาเป้าหมายแบบตรง ๆ
        Vector3 dir = (ctx.shipTarget.position - ctx.transform.position).normalized;
        ctx.transform.position += dir * ctx.moveSpeed * Time.deltaTime;
        Vector3 lookDir = ctx.shipTarget.position - ctx.transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            ctx.transform.rotation = Quaternion.Slerp(ctx.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // โจมตีถ้าเข้าใกล้
        if (ctx.IsShipInAttackRange() && ctx.IsAttackCooldownReady())
        {
            ctx.StateMachine.ChangeState(ctx.attackIdleState);
        }
    }

    public void Exit()
    {
        ctx.animator.SetBool("IsChasing", false);
    }
}
