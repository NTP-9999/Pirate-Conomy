public class KravalonStateMachine
{
    private IKravalonState currentState;

    public void Initialize(IKravalonState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(IKravalonState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState.Execute();
    }
}
