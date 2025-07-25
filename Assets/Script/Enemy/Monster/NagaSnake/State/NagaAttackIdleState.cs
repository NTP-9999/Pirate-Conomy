using UnityEngine;

public class NagaAttackIdleState : NagaIState
{
    private SnakeNagaAI ctx;
    private float enterTime;
    private float idleBeforeAttack = 0.8f; // รอประมาณ 0.8 วิ

    public NagaAttackIdleState(SnakeNagaAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        ctx.Agent.isStopped = true;
        enterTime = Time.time;
    }

    public void Execute()
    {
        // ถ้าผู้เล่นออกจากระยะก่อนครบเวลา → กลับไปไล่
        if (!ctx.IsPlayerInAttackRange())
        {
            ctx.StateMachine.ChangeState(ctx.chaseState);
            return;
        }

        // ถ้าครบเวลา → เข้าสู่โจมตีจริง
        if (Time.time >= enterTime + idleBeforeAttack)
        {
            ctx.StateMachine.ChangeState(ctx.attackState);
        }
    }

    public void Exit() { }
}
