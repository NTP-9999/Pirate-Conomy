// CrabHurtState.cs
using UnityEngine;

public class CrabHurtState : CrabIState
{
    private readonly CrabAI ctx;
    private float startTime;

    public CrabHurtState(CrabAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        ctx.Agent.isStopped = true;
        ctx.Animator.SetTrigger("Hurt");
        startTime = Time.time;
    }

    public void Execute()
    {
        if (Time.time >= startTime + ctx.hurtDuration)
        {
            if (ctx.IsPlayerInAttackRange())
                ctx.StateMachine.ChangeState(ctx.attackState);
            else if (ctx.IsPlayerInChaseRange())
                ctx.StateMachine.ChangeState(ctx.chaseState);
            else
                ctx.StateMachine.ChangeState(ctx.hideState);
        }
    }

    public void Exit() { }
}
