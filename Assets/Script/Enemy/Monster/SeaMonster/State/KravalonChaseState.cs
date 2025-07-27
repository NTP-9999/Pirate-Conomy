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

        float distance = Vector3.Distance(ctx.transform.position, ctx.shipTarget.position);
        if (distance > ctx.stoppingDistance)
        {
            // เดินเข้าเป้าหมาย
            Vector3 moveDir = (ctx.shipTarget.position - ctx.transform.position).normalized;
            ctx.transform.position += moveDir * ctx.moveSpeed * Time.deltaTime;
        }

        // **ใช้เมธอดเดียวกันทุก state**
        

        if (distance <= ctx.attackRange && ctx.IsAttackCooldownReady())
            ctx.StateMachine.ChangeState(ctx.attackState);
    }

    public void Exit()
    {
        ctx.animator.SetBool("IsChasing", false);
    }
}
