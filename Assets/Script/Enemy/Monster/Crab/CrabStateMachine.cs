// CrabStateMachine.cs
public class CrabStateMachine
{
    public CrabIState CurrentState { get; private set; }

    public void Initialize(CrabIState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(CrabIState newState)
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
