using UnityEngine;
public class PlayerJumpState : IState
{
    PlayerStateMachine sm;
    private float timer;
    private readonly float minJumpTime = 0.1f; // ให้เล่นอนิเมชันอย่างน้อยนิดหน่อยก่อนเช็คตกพื้น
    private bool wasGroundedLastFrame = true;
    

    public PlayerJumpState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        
        Debug.Log("JumpState Entered");
        var pcCheck = sm.playerController;
        if (pcCheck.isParryActive)
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }
        var stats = CharacterStats.Instance;
        var pc = sm.playerController;
        if (stats.currentStamina < pc.jumpStaminaCost)
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }

        // ตัด Stamina
        stats.UseStamina(pc.jumpStaminaCost);
        pc.skipGroundSnap = true;

        // 1) สั่งกระโดด
        pc.Jump();
        PlayerAudioManager.Instance?.PlayOneShot(PlayerAudioManager.Instance.jumpClip);

        // 2) สั่งรีเซ็ตและป้องกันไม่ให้ Exit ทันที
        timer = 0f;
        wasGroundedLastFrame = true;
        pc.animator.SetTrigger("Jump");
    }

    public void Execute()
    {
        var pc = sm.playerController;

        // 1) ให้มันอัปเดต Movement+Gravity ตลอด (อัปเดต MoveX/MoveZ/IsRunning/IsGrounded)
        pc.HandleMovement();

        // 2) นับเวลาอย่างน้อยก่อนจะอนุญาตให้กลับ Idle
        timer += Time.deltaTime;
        bool isGrounded = pc.IsGrounded();

        // Detect land: false → true
        if (!wasGroundedLastFrame && isGrounded)
        {
            Debug.Log("Landed!");
            PlayerAudioManager.Instance?.PlayOneShot(PlayerAudioManager.Instance.landClip);
        }

        // Update last grounded state
        wasGroundedLastFrame = isGrounded;

        if (timer < minJumpTime) return;

        if (isGrounded)
        {
            sm.fsm.ChangeState(sm.idleState);
        }
    }

    public void Exit()
    {
        sm.playerController.skipGroundSnap = false;
    }
}
