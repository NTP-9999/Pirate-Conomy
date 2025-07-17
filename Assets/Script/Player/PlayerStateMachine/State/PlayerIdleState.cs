using UnityEngine;

public class PlayerIdleState : IState
{
    PlayerStateMachine sm;
    public PlayerIdleState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        sm.playerController.canMove = true;
        sm.playerController.animator.SetBool("IsMoving", false);
    }

    public void Execute()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool moving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded())
        {
            sm.fsm.ChangeState(sm.jumpState);
            return;
        }

        // If skill is locked, only allow walking
        if (sm.playerController.isSkillLocked)
        {
            if (moving)
                sm.fsm.ChangeState(sm.moveState);
            return;
        }

        // Walk / run
        if (moving)
        {
            sm.fsm.ChangeState(sm.moveState);
            return;
        }

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            sm.fsm.ChangeState(sm.attackState);
            return;
        }

        // Interact / Collect
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (sm.currentOil != null)
            {
                sm.fsm.ChangeState(sm.collectOilState);
                return;
            }
            if (sm.currentOre != null)
            {
                sm.fsm.ChangeState(sm.collectOreState);
                return;
            }
            if (sm.currentTree != null)
            {
                sm.fsm.ChangeState(sm.collectTreeState);
                return;
            }
            if (sm.currentFragment != null)
            {
                sm.fsm.ChangeState(sm.collectFragmentState);
                return;
            }
        }
    }

    public void Exit() { }
}
