using UnityEngine;

public class KravalonChaseState : IKravalonState
{
    private KravalonAI ctx;

    public KravalonChaseState(KravalonAI context)
    {
        ctx = context;
    }

    public void Enter() { }

    public void Execute()
    {
        if (ctx.shipTarget == null) return;

        Vector3 dir = (ctx.shipTarget.position - ctx.transform.position).normalized;
        ctx.transform.position += dir * ctx.moveSpeed * Time.deltaTime;

        if (ctx.IsShipInAttackRange() && ctx.IsAttackCooldownReady())
        {
            ctx.StateMachine.ChangeState(ctx.attackIdleState);
        }
    }

    public void Exit() { }
}
