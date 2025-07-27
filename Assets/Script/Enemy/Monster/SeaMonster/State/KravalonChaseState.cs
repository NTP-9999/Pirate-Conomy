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
            Vector3 dir = (ctx.shipTarget.position - ctx.transform.position).normalized;
            ctx.transform.position += dir * ctx.moveSpeed * Time.deltaTime;
        }

        // หมุนหาทิศเรือ
        Vector3 lookDir = ctx.shipTarget.position - ctx.transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            ctx.transform.rotation = Quaternion.Slerp(ctx.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // เข้า attackIdle เมื่ออยู่ในระยะโจมตีและคูลดาวน์พร้อม
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
