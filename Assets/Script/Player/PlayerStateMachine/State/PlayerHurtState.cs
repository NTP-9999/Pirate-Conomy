using UnityEngine;

public class PlayerHurtState : IState
{
    PlayerStateMachine sm;
    float hurtDuration = 0.7f; // ระยะเวลาที่ถูก stun
    float timer;

    public PlayerHurtState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        Debug.Log("Player Enter Hurt State");
        timer = 0f;
        sm.playerController.animator.SetTrigger("Hurt");
        // ปิดการเคลื่อนไหว
        sm.playerController.canMove = false;
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= hurtDuration)
        {
            // กลับไป idle หรือ move
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                sm.fsm.ChangeState(sm.moveState);
            else
                sm.fsm.ChangeState(sm.idleState);
        }
    }

    public void Exit()
    {
        Debug.Log("Player Exit Hurt State");
        // กลับมาให้เคลื่อนไหวได้
        sm.playerController.canMove = true;
    }
}
