using UnityEngine;
public class PlayerSwimState : IState
{
    PlayerStateMachine sm;
    public PlayerSwimState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        Debug.Log("Enter Swim State");
        sm.playerController.animator.SetBool("Swimming", true);
    }

    public void Execute()
    {
        // ให้มันเคลื่อนที่เหมือนบนบก (แต่ Animator ปรับเป็น Swim)
        sm.playerController.HandleMovement();
    }

    public void Exit()
    {
        sm.playerController.animator.SetBool("Swimming", false);
    }
}
