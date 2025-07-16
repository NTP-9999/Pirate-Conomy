using UnityEngine;
public class PlayerJumpState : IState
{
    PlayerStateMachine sm;
    private float timer;
    private readonly float minJumpTime = 0.1f; // ให้เล่นอนิเมชันอย่างน้อยนิดหน่อยก่อนเช็คตกพื้น

    public PlayerJumpState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        var stats = CharacterStats.Instance;
        var pc = sm.playerController;
        if (stats.currentStamina < pc.jumpStaminaCost)
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }

        // ตัด Stamina
        stats.UseStamina(pc.jumpStaminaCost);

        // 1) สั่งกระโดด
        pc.Jump();

        // 2) สั่งรีเซ็ตและป้องกันไม่ให้ Exit ทันที
        timer = 0f;

        pc.animator.SetBool("IsMoving", true);
        pc.animator.SetTrigger("Jump");
    }

    public void Execute()
    {
        var pc = sm.playerController;

        // 1) ให้มันอัปเดต Movement+Gravity ตลอด (อัปเดต MoveX/MoveZ/IsRunning/IsGrounded)
        pc.HandleMovement();

        // 2) นับเวลาอย่างน้อยก่อนจะอนุญาตให้กลับ Idle
        timer += Time.deltaTime;
        if (timer < minJumpTime) return;

        // 3) พอแตะพื้น → กลับ Idle  
        if (pc.IsGrounded())
            sm.fsm.ChangeState(sm.idleState);
    }

    public void Exit()
    {
        // ไม่มีอะไรต้องเคลียร์พิเศษ
    }
}
