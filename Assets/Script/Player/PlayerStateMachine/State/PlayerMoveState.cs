using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Player Enter Move State");
        stateMachine.playerController.animator.SetBool("IsMoving", true); // ✅ ดึงจาก PlayerController
    }

    public override void LogicUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;

        // อัปเดตค่าพารามิเตอร์ใน Animator ผ่าน PlayerController
        stateMachine.playerController.animator.SetFloat("MoveX", moveX);
        stateMachine.playerController.animator.SetFloat("MoveZ", moveZ);

        // ตรวจว่าผู้เล่นยังเคลื่อนที่อยู่ไหม
        bool isMoving = (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0);
        stateMachine.playerController.animator.SetBool("IsMoving", isMoving);
        if (Input.GetKeyDown(KeyCode.Space) && stateMachine.playerController.IsGrounded() && isMoving)
        {
            stateMachine.ChangeState(stateMachine.jumpState);
        }
        if (!isMoving)
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }

        if (Input.GetMouseButtonDown(0))
        {
            stateMachine.ChangeState(stateMachine.attackState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Player Exit Move State");
    }
}
