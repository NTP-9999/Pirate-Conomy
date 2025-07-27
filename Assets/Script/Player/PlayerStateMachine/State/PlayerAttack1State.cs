// PlayerAttack1State.cs
using UnityEngine;

public class PlayerAttack1State : IState
{
    PlayerStateMachine sm;
    float timer;

    public PlayerAttack1State(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        timer = 0f;
        sm.comboInputBuffered = false;
        PlayerAudioManager.Instance.PlayOneShot(PlayerAudioManager.Instance.attack1Clip);
        sm.playerController.animator.SetTrigger("Attack1");
        Debug.Log("PlayerAttack1State: Entered");
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            sm.comboInputBuffered = true;
            Debug.Log("[Attack1] Mouse Clicked! comboInputBuffered = " + sm.comboInputBuffered);
        }
        if (timer >= sm.maxComboDelay)
            {
                if (sm.comboInputBuffered)
                {
                    sm.fsm.ChangeState(sm.attack2State);
                    Debug.Log("[Attack1] Combo buffered, changing to Attack2State");
                }
                else
                    sm.fsm.ChangeState(sm.idleState);
            }
        
    }

    public void Exit()
    {
        Debug.Log("PlayerAttack1State: Exited");
        sm.playerController.animator.ResetTrigger("Attack1");
    }
}
