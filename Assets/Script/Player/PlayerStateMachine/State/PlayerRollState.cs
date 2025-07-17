using UnityEngine;

public class PlayerRollState : IState
{
    private PlayerStateMachine sm;
    private Vector3 dir;
    private float elapsed;
    private readonly float duration = 0.5f;

    public PlayerRollState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        var pc = sm.playerController;
        Debug.Log("→ Enter RollState, trigger sent");
        sm.playerController.canMove = false;
        var stats = CharacterStats.Instance;
        if (stats.currentStamina < pc.rollStaminaCost)
        {
            sm.fsm.ChangeState(sm.idleState);
            return;
        }

        // ตัด Stamina
        stats.UseStamina(pc.rollStaminaCost);


        // อ่าน Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // ตั้ง Animator Parameters ให้ Blend Tree เลือกคลิปถูก
        var anim = sm.playerController.animator;
        anim.SetFloat("MoveX", h);
        anim.SetFloat("MoveZ", v);

        // สั่งเล่นอนิเมชั่น Roll (เข้า Blend Tree นี้)
        anim.SetTrigger("Roll");

        // เก็บ dir สำหรับการเคลื่อนตัวจริง
        Vector3 inDir = new Vector3(h, 0, v).normalized;
        dir = inDir.magnitude > 0.1f 
            ? sm.playerController.transform.TransformDirection(inDir) 
            : sm.playerController.transform.forward;

        elapsed = 0f;
    }


    public void Execute()
    {
        // เลื่อนตัวตามทิศทาง + ความเร็ว rollSpeed
        sm.playerController.Move(dir * sm.playerController.rollSpeed * Time.deltaTime);

        // อัปเดตเวลา
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            // ครบเวลาแล้ว กลับไป Idle
            sm.fsm.ChangeState(sm.idleState);
        }
    }

    public void Exit()
    {
        // คืนสิทธิ์ movement ปกติ
        sm.playerController.canMove = true;
    }
}
