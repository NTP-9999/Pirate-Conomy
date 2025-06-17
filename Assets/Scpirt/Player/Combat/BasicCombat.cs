using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Dependencies")]
    // [SerializeField] private PlayerWeaponManager playerWeaponManager; // เราไม่ใช้ตรงนี้แล้ว ให้คอมเมนต์หรือลบไป
    [Tooltip("Reference to the Player Animator Controller.")]
    [SerializeField] private Animator playerAnimator; // ยังคงใช้ Animator เพื่อเช็คสถานะหรือควบคุมอื่นๆ ที่ไม่เกี่ยวกับการโจมตีโดยตรง

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 0.5f; // Cooldown สำหรับการโจมตี
    private float nextAttackTime = 0f;
    public bool CanAttack
    {
        get
        {
            bool onCooldown = Time.time < nextAttackTime;
            return !onCooldown;
        }
    }

    void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("BasicCombat: Animator not found on this GameObject. Please assign it.", this);
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// เมธอดสำหรับสั่งให้เริ่ม Cooldown การโจมตี
    /// </summary>
    public void TriggerAttackCooldown()
    {
        nextAttackTime = Time.time + attackCooldown;
    }
}