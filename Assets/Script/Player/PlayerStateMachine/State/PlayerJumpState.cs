using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private PlayerController playerController;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        playerController = stateMachine.playerController;
    }

    public override void Enter()
    {
        Debug.Log("Enter Jump State");

        // สั่ง PlayerController กระโดด
        playerController.Jump();

        // เล่นแอนิเมชัน
        playerController.animator.SetTrigger("Jump");
    }

    public override void LogicUpdate()
    {
        // ตรวจว่าตกถึงพื้นหรือยัง
        if (playerController.IsGrounded())
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }

        // ระหว่างลอยอยู่ สามารถตรวจ input เพิ่มเติมได้ถ้าอยากทำ (เช่น สั่งโจมตีกลางอากาศ)
    }

    public override void Exit()
    {
        Debug.Log("Exit Jump State");
    }
}
