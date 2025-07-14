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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool moving = Mathf.Abs(h) > 0f || Mathf.Abs(v) > 0f;

        // If skill is locked, only allow walking
        if (sm.playerController.isSkillLocked)
        {
            sm.playerController.HandleMovement();
            if (!moving)
                sm.fsm.ChangeState(sm.idleState);
            return;
        }

        // Allow dodge/roll even while moving
        if (Input.GetKeyDown(KeyCode.Q))
        {
            sm.fsm.ChangeState(sm.rollState);
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

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded())
        {
            sm.fsm.ChangeState(sm.jumpState);
            return;
        }

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            sm.fsm.ChangeState(sm.attackState);
            return;
        }
    }

    public void Exit()
    {
        sm.playerController.animator.SetBool("IsMoving", false);
    }
}
