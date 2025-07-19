// PlayerAttackState.cs
using UnityEngine;

public class PlayerAttackState : IState
{
    PlayerStateMachine sm;
    float timer;

    public PlayerAttackState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        // 1) เลื่อนสเต็ปคอมโบ
        sm.comboStep = Mathf.Clamp(sm.comboStep + 1, 1, sm.maxComboStep);
        sm.comboInputBuffered = false;
        timer = 0f;

        // 2) ตั้ง Animator params (ให้ Blend Tree เลือกคลิปตาม ComboStep)
        sm.playerController.animator.SetFloat("ComboStep", sm.comboStep);
        sm.playerController.animator.SetTrigger("Attack");

        // 3) Deal Damage (หรือจะใช้ Animation Event แทนก็ได้)
        sm.playerController.DealDamage();
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        // 4) Buffer input ซ้ำระหว่างอนิเมชัน
        if (Input.GetButtonDown("Fire1"))
            sm.comboInputBuffered = true;

        // 5) ตรวจเวลา หากถึงก็ไปขั้นถัดไปหรือจบคอมโบ
        if (timer >= sm.maxComboDelay)
        {
            if (sm.comboInputBuffered && sm.comboStep < sm.maxComboStep)
            {
                sm.fsm.ChangeState(sm.attackState);  
            }
            else
            {
                // รีเซ็ตคอมโบ + กลับ Idle
                sm.comboStep = 0;
                sm.playerController.animator.SetFloat("ComboStep", 0);
                sm.fsm.ChangeState(sm.idleState);
            }
        }
    }

    public void Exit()
    {
        // เคลียร์ Trigger เผื่อค้าง
        sm.playerController.animator.ResetTrigger("Attack");
    }
}
