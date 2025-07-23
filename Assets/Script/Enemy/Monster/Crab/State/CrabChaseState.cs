// CrabChaseState.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrabChaseState : CrabIState
{
    private readonly CrabAI ctx;
    private readonly NavMeshAgent agent;

    public CrabChaseState(CrabAI context)
    {
        ctx = context;
        agent = ctx.Agent;
    }

    public void Enter()
    {
        ctx.Animator.SetTrigger("Chasing"); // ตั้ง Trigger ตามที่คุณตั้งไว้ใน Animator
        ctx.StartCoroutine(StartChaseAfterDelay());
        agent.isStopped = false;
    }

    public void Execute()
    {
        // ไล่ผู้เล่น
        agent.SetDestination(ctx.PlayerPosition);

        // ถ้าเข้า Attack range สลับไปตี
        if (ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.attackState);
        // ถ้าเลย Detection range กลับไปซ่อนใหม่
        else if (!ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.hideState);
    }

    public void Exit()
    {
    }
    private IEnumerator StartChaseAfterDelay()
    {
        yield return new WaitForSeconds(1f);
    }
}
