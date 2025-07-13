using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float timer;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Player Enter Attack State");
        timer = 0f;
        stateMachine.playerController.animator.SetTrigger("Attack");
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= attackDuration)
        {
            // กลับไป idle หรือ move ตาม input
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                stateMachine.ChangeState(stateMachine.moveState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Player Exit Attack State");
    }
}
