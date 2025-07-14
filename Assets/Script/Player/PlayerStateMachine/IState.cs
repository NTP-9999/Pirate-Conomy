public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class StateMachine
{
    private IState _current;
    public void Initialize(IState startingState)
    {
        _current = startingState;
        _current.Enter();
    }
    public void ChangeState(IState next)
    {
        _current.Exit();
        _current = next;
        _current.Enter();
    }
    public void Update() => _current?.Execute();
}
