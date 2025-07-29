using UnityEngine;

public class PlayerMoveState : IState
{
    PlayerStateMachine sm;
    public PlayerMoveState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        sm.playerController.animator.SetBool("IsMoving", true);
    }

    public void Execute()
    {
        
          // Attack
        if (Input.GetMouseButtonDown(0))
        {
            sm.fsm.ChangeState(sm.attack1State);
            return;
        }
        // Allow dodge/roll even while moving
        if (Input.GetKeyDown(KeyCode.Q) && sm.playerController.CanRoll())
        {
            sm.fsm.ChangeState(sm.rollState);
            return;
        }
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded())
        {
            sm.fsm.ChangeState(sm.jumpState);
            return;
        }
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool moving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // If skill is locked, only allow walking
        if (PlayerAudioManager.Instance != null && moving && sm.playerController.IsGrounded())
        {
            PlayerAudioManager.Instance.PlayFootstep(sm.playerController.isRunning);
        }
        if (sm.playerController.isSkillLocked)
        {
            sm.playerController.HandleMovement();
            if (!moving)
                sm.fsm.ChangeState(sm.idleState);
            return;
        }
        // Normal movement
        sm.playerController.HandleMovement();

        // Stop moving → back to idle
        if (!moving)
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }
    }

    public void Exit()
    {
        sm.playerController.animator.SetBool("IsMoving", false);
    }
}
