// ChaseState.cs


public class NagaChaseState : NagaIState {
    SnakeNagaAI ctx;
    UnityEngine.AI.NavMeshAgent agent;

    public NagaChaseState(SnakeNagaAI context) {
        ctx = context;
        agent = ctx.Agent;
    }

    public void Enter() { agent.isStopped = false; }

    public void Execute() {
        agent.SetDestination(ctx.PlayerPosition);

        if (ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.attackState);
        else if (!ctx.IsPlayerInChaseRange())
            ctx.StateMachine.ChangeState(ctx.patrolState);
    }

    public void Exit() { }
}
