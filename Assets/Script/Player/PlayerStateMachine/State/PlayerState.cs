public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;

    protected PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() {}
    public virtual void LogicUpdate() {}
    public virtual void Exit() {}
}
