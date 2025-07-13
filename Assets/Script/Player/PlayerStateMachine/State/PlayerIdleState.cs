using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Player Enter Idle State");
        // เล่นแอนิเมชัน idle ถ้ามี
    }

    public override void LogicUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        bool isMoving = (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0);
        if (Input.GetKeyDown(KeyCode.Space) && stateMachine.playerController.IsGrounded() && isMoving)
        {
            stateMachine.ChangeState(stateMachine.jumpState);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            stateMachine.ChangeState(stateMachine.moveState);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            stateMachine.ChangeState(stateMachine.attackState);
        }
    }


    public override void Exit()
    {
        Debug.Log("Player Exit Idle State");
    }
}
