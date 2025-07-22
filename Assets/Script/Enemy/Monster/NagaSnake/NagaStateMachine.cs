using UnityEngine;
public class NagaStateMachine
{
    public NagaIState CurrentState { get; private set; }

    public void Initialize(NagaIState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(NagaIState newState)
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
