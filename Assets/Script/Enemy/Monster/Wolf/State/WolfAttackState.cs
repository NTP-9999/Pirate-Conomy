// WolfAttackState.cs
using UnityEngine;

public class WolfAttackState : WolfIState
{
    private WolfAI ctx;
    private float lastAtkTime;

    public WolfAttackState(WolfAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        ctx.Agent.isStopped = true;
    }

    public void Execute()
    {
        ctx.RotateTowardsPlayer();

        if (Time.time >= lastAtkTime + ctx.timeBetweenAttacks)
        {
            // สุ่มท่าโจมตี 1–3
            int choice = Random.Range(1, 4);
            ctx.Animator.SetTrigger("Attack" + choice);
            lastAtkTime = Time.time;
        }

        if (!ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit(){}

}
