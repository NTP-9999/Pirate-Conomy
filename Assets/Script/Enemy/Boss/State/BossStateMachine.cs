public class BossStateMachine
{
    public BossIState CurrentState { get; private set; }

    public void ChangeState(BossIState newState)
    {
        if (CurrentState != null) CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }
}
