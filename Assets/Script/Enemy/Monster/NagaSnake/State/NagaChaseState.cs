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
            ctx.StateMachine.ChangeState(ctx.attackIdleState);
        else if (!ctx.IsPlayerInChaseRange())
        {
            ctx.StateMachine.ChangeState(ctx.patrolState);
            NagaAudioManager.Instance.StopLoop();
        }
            
    }

    public void Exit() { }
}
