// CrabAttackState.cs
using UnityEngine;

public class CrabAttackState : CrabIState
{
    private CrabAI ctx;
    private float lastAtkTime;

    public CrabAttackState(CrabAI context)
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
            int choice = Random.Range(1, 4);  // สุ่ม 1–3
            ctx.Animator.SetTrigger("Attack" + choice);
            lastAtkTime = Time.time;
        }

        if (!ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit() { }
}
