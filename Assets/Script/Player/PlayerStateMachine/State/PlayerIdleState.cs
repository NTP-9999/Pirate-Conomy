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
        // 1) Movement
        float h = Input.GetAxisRaw("Horizontal"),
              v = Input.GetAxisRaw("Vertical");
        bool moving = (Mathf.Abs(h)>0||Mathf.Abs(v)>0);
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded()) { sm.fsm.ChangeState(sm.jumpState); return; }
        if (moving) { sm.fsm.ChangeState(sm.moveState); return; }

        // 2) Attack
        if (Input.GetMouseButtonDown(0)) { sm.fsm.ChangeState(sm.attackState); return; }

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
            // 5) Interact E → Fragment
            if (sm.currentFragment != null) {
                sm.fsm.ChangeState(sm.collectFragmentState);
                return;
            }
        }
    }
    public void Exit() { }
}
