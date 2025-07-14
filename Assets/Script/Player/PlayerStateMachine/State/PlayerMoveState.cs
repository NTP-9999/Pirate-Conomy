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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            sm.fsm.ChangeState(sm.rollState);
            return;
        }
        // copy–paste HandleMovement แต่กรอง input แค่ XZ
        sm.playerController.HandleMovement(); // (ย้ายโค้ด HandleMovement มา public แล้วเรียกใช้)
        
        // ถ้าเลิกขยับ → กลับ Idle
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }
        
        // พ่วง Jump/Attack เหมือน Idle ก็ได้
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded())
            sm.fsm.ChangeState(sm.jumpState);
        if (Input.GetMouseButtonDown(0))
            sm.fsm.ChangeState(sm.attackState);
    }
    public void Exit()
    {
        sm.playerController.animator.SetBool("IsMoving", false);
    }
    
}

