// AttackState.cs
using UnityEngine;

public class NagaAttackState : NagaIState {
    SnakeNagaAI ctx;
    float lastAtkTime;

    public NagaAttackState(SnakeNagaAI context) {
        ctx = context;
    }

    public void Enter() {
        lastAtkTime = Time.time - ctx.timeBetweenAttacks;
        ctx.Agent.isStopped = true;
    }

    public void Execute() {
        ctx.RotateTowardsPlayer();

        if (Time.time >= lastAtkTime + ctx.timeBetweenAttacks) {
            int atk = Random.Range(1, 4);
            ctx.Animator.SetTrigger("Attack" + atk);
            lastAtkTime = Time.time;
        }

        if (!ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit() { }
}
