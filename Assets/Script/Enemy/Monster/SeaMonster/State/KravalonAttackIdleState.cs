using UnityEngine;

public class KravalonAttackIdleState : IKravalonState
{
    private KravalonAI ctx;
    private float enterTime;

    public KravalonAttackIdleState(KravalonAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        enterTime = Time.time;
    }

    public void Execute()
    {
        // ถ้าผู้เล่นหนีออกจากระยะก่อนครบเวลา → ไล่ต่อ
        if (!ctx.IsShipInAttackRange())
        {
            ctx.StateMachine.ChangeState(ctx.chaseState);
            return;
        }

        // ถ้ารอครบ → โจมตี
        if (Time.time >= enterTime + ctx.idleBeforeAttack)
        {
            ctx.StateMachine.ChangeState(ctx.attackState);
        }
    }

    public void Exit() { }
}
