using UnityEngine;

public class PlayerHurtState : PlayerState
{
    private float hurtDuration = 0.7f; // ระยะเวลาที่ถูก stun
    private float timer;

    public PlayerHurtState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Player Enter Hurt State");
        timer = 0f;
        stateMachine.playerController.animator.SetTrigger("Hurt");
        // ปิดการเคลื่อนไหว
        stateMachine.playerController.canMove = false;
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= hurtDuration)
        {
            // กลับไป idle หรือ move
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                stateMachine.ChangeState(stateMachine.moveState);
            else
                stateMachine.ChangeState(stateMachine.idleState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Player Exit Hurt State");
        // กลับมาให้เคลื่อนไหวได้
        stateMachine.playerController.canMove = true;
    }
}
