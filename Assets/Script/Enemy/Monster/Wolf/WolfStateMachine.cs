// WolfStateMachine.cs
public class WolfStateMachine
{
    public WolfIState CurrentState { get; private set; }

    public void Initialize(WolfIState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(WolfIState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState.Execute();
    }
}
