using UnityEngine;

public class BossAttackStateBehaviour : StateMachineBehaviour
{
    private BossAI bossAI; 
    private BossAttackCollider bossAttackCollider;
    private bool isHitboxActive = false; 

    [System.Serializable]
    public struct AttackWindow
    {
        [Range(0f, 1f)] public float startNormalizedTime;
        [Range(0f, 1f)] public float endNormalizedTime;
    }

    public AttackWindow[] attackWindows;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (bossAI == null)
        {
            bossAI = animator.GetComponentInParent<BossAI>(); 
        }
        if (bossAttackCollider == null)
        {
            bossAttackCollider = animator.GetComponentInChildren<BossAttackCollider>();
        }
        
        if (bossAI != null)
        {
            bossAI.SetAttackingState(true); 
        }
        else
        {
            Debug.LogError("BossAI not found in OnStateEnter for BossAttackStateBehaviour!");
        }

        if (bossAttackCollider != null)
        {
            bossAttackCollider.DisableHitbox(); // ตรวจสอบให้แน่ใจว่าปิด hitbox ไว้ก่อน
            isHitboxActive = false;
            Debug.Log($"Attack State Enter: Disabled hitbox. Normalized Time: {stateInfo.normalizedTime}"); 
        }
        else
        {
            Debug.LogError("BossAttackCollider not found in OnStateEnter!");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (bossAttackCollider == null) return;

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

        // === ควบคุม Hitbox ===
        if (shouldBeActive && !isHitboxActive)
        {
            bossAttackCollider.EnableHitbox();
            isHitboxActive = true;
            Debug.Log($"Hitbox Activated at Normalized Time: {currentNormalizedTime}"); 
        }
        else if (!shouldBeActive && isHitboxActive)
        {
            bossAttackCollider.DisableHitbox();
            isHitboxActive = false;
            Debug.Log($"Hitbox Deactivated at Normalized Time: {currentNormalizedTime}"); 
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (bossAttackCollider != null)
        {
            bossAttackCollider.DisableHitbox();
            bossAttackCollider.ResetDamageStatus(); // รีเซ็ตสถานะการทำดาเมจเพื่อให้โจมตีครั้งต่อไปได้
            isHitboxActive = false;
            Debug.Log($"Attack State Exit: Disabled hitbox and reset damage status. Normalized Time: {stateInfo.normalizedTime}"); 
        }

        if (bossAI != null)
        {
            bossAI.SetAttackingState(false);
        }
        else
        {
            Debug.LogError("BossAI not found in OnStateExit for BossAttackStateBehaviour!");
        }
    }
}