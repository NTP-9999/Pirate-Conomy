using UnityEngine;

public class PlayerAttackStateBehaviour : StateMachineBehaviour
{
    private PlayerAttackCollider playerAttackCollider;
    private BasicCombat playerCombat; // อ้างอิงถึง BasicCombat
    private bool isHitboxActive = false;

    [System.Serializable]
    public struct AttackWindow
    {
        [Range(0f, 1f)] public float startNormalizedTime;
        [Range(0f, 1f)] public float endNormalizedTime;
    }

    public AttackWindow[] attackWindows;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // หากยังไม่ได้อ้างอิง
        if (playerAttackCollider == null)
        {
            playerAttackCollider = animator.GetComponentInChildren<PlayerAttackCollider>();
            if (playerAttackCollider == null)
            {
                Debug.LogError("PlayerAttackCollider not found in children of " + animator.name + "!");
            }
        }
        if (playerCombat == null)
        {
            playerCombat = animator.GetComponent<BasicCombat>(); // BasicCombat อยู่บน Player GameObject เดียวกันกับ Animator
            if (playerCombat == null)
            {
                Debug.LogError("BasicCombat script not found on " + animator.name + "!");
            }
        }

        // เมื่อเข้าสู่สถานะแอนิเมชันโจมตี
        if (playerCombat != null)
        {
            playerCombat.isAttacking = true; // หยุดการเคลื่อนที่ของผู้เล่น
        }
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox(); // ตรวจสอบให้แน่ใจว่าปิดอยู่ก่อนเริ่ม
            isHitboxActive = false;
            playerAttackCollider.ResetDamageDealt(); // รีเซ็ตสถานะการทำดาเมจสำหรับคอมโบใหม่
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAttackCollider == null) return;

        float currentNormalizedTime = stateInfo.normalizedTime % 1f;

        bool shouldBeActive = false;
        foreach (var window in attackWindows)
        {
            if (currentNormalizedTime >= window.startNormalizedTime && currentNormalizedTime < window.endNormalizedTime)
            {
                shouldBeActive = true;
                break;
            }
        }

        if (shouldBeActive && !isHitboxActive)
        {
            playerAttackCollider.EnableHitbox();
            isHitboxActive = true;
        }
        else if (!shouldBeActive && isHitboxActive)
        {
            playerAttackCollider.DisableHitbox();
            isHitboxActive = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // เมื่อออกจากสถานะแอนิเมชันโจมตี
        if (playerCombat != null)
        {
            playerCombat.isAttacking = false; // อนุญาตให้ผู้เล่นเคลื่อนที่ได้อีกครั้ง
        }
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
            isHitboxActive = false;
        }
    }
}