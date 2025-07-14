using UnityEngine;
public class PlayerAttackState : IState
{
    PlayerStateMachine sm;
    float timer, duration = 0.5f;
    public PlayerAttackState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        timer = 0f;
        sm.playerController.animator.SetTrigger("Attack");
    }
    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            sm.fsm.ChangeState(sm.idleState);
    }
    public void Exit() { }
}
