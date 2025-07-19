// PlayerAttack3State.cs
using UnityEngine;

public class PlayerAttack3State : IState
{
    PlayerStateMachine sm;
    float timer;

    public PlayerAttack3State(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        timer = 0f;
        sm.comboInputBuffered = false;
        sm.playerController.animator.SetTrigger("Attack3");
        sm.playerController.DealDamage();
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        // ไม่มีท่าต่อไป หลังจบคอมโบก็กลับ Idle
        if (timer >= sm.maxComboDelay)
            sm.fsm.ChangeState(sm.idleState);
    }

    public void Exit()
    {
        sm.playerController.animator.ResetTrigger("Attack3");
    }
}
