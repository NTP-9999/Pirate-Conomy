// PlayerAttack2State.cs
using UnityEngine;

public class PlayerAttack2State : IState
{
    PlayerStateMachine sm;
    float timer;

    public PlayerAttack2State(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        timer = 0f;
        sm.comboInputBuffered = false;
        sm.playerController.animator.SetTrigger("Attack2");
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
            sm.comboInputBuffered = true;

        if (timer >= sm.maxComboDelay)
        {
            if (sm.comboInputBuffered)
                sm.fsm.ChangeState(sm.attack3State);
            else
                sm.fsm.ChangeState(sm.idleState);
        }
    }

    public void Exit()
    {
        sm.playerController.animator.ResetTrigger("Attack2");
    }
}
