// WolfChaseState.cs
using UnityEngine;
using UnityEngine.AI;

public class WolfChaseState : WolfIState
{
    private WolfAI ctx;
    private NavMeshAgent agent;
    private float speedmultiplier = 1.5f;

    public WolfChaseState(WolfAI context)
    {
        ctx = context;
        agent = ctx.Agent;
    }

    public void Enter()
    {
        agent.isStopped = false;

        ctx.Animator.SetBool("isChasing", true);
        agent.speed = agent.speed * speedmultiplier; // Increase speed when chasing
    }

    public void Execute()
    {
        agent.SetDestination(ctx.PlayerPosition);

        if (ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.attackState);
        else if (!ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.patrolState);
    }

    public void Exit()
    {
        ctx.Animator.SetBool("isChasing", false);
        agent.speed = agent.speed / speedmultiplier; // Reset speed after chasing
    }
}
