// WolfHurtState.cs
using UnityEngine;

public class WolfHurtState : WolfIState
{
    private WolfAI ctx;
    private float startTime;

    public WolfHurtState(WolfAI context)
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
                ctx.StateMachine.ChangeState(ctx.patrolState);
        }
    }

    public void Exit() { }
}
