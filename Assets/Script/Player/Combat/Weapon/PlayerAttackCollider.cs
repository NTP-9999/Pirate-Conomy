using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 10;

    private Collider playerWeaponCollider;
    private bool hasDealtDamageThisAttackInstance = false; // เพื่อให้ทำดาเมจแค่ครั้งเดียวต่อการโจมตี 1 ครั้ง

    void Awake()
    {
        playerWeaponCollider = GetComponent<Collider>();
        if (playerWeaponCollider == null)
        {
            Debug.LogError("PlayerAttackCollider requires a Collider component on the same GameObject!");
        }
        else
        {
            if (!playerWeaponCollider.isTrigger) playerWeaponCollider.isTrigger = true;
            playerWeaponCollider.enabled = false; // เริ่มต้นปิด Collider ไว้
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else if (!rb.isKinematic)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void EnableHitbox()
    {
        if (playerWeaponCollider != null)
        {
            playerWeaponCollider.enabled = true;
            hasDealtDamageThisAttackInstance = false;
            Debug.Log($"[Hitbox] {gameObject.name} Enabled");
        }
    }

    public void DisableHitbox()
    {
        if (playerWeaponCollider != null)
        {
            playerWeaponCollider.enabled = false;
            Debug.Log($"[Hitbox] {gameObject.name} Disabled");
        }
    }

    public void ResetDamageDealt()
    {
        hasDealtDamageThisAttackInstance = false;
    }
    // ************************************************************

    void OnTriggerEnter(Collider other)
    {
        if (playerWeaponCollider.enabled && !hasDealtDamageThisAttackInstance)
        {
            // 1. ตรวจสอบ Boss (ใช้ BossStats)
            if (other.CompareTag("Boss"))
            {
                BossStats bossStats = other.GetComponent<BossStats>();
                if (bossStats != null && !bossStats.IsDead)
                {
                    bossStats.TakeDamage(attackDamage);
                    hasDealtDamageThisAttackInstance = true;
                    Debug.Log("Player dealt " + attackDamage + " damage to Boss: " + other.name);
                    return;
                }
            }

            // 2. ตรวจสอบ Enemy (ใช้ LivingEntityStats)
            if (other.CompareTag("Enemy"))
            {
                LivingEntityStats enemyStats = other.GetComponent<LivingEntityStats>();
                if (enemyStats != null && !enemyStats.IsDead)
                {
                    enemyStats.TakeDamage(attackDamage);
                    hasDealtDamageThisAttackInstance = true;
                    Debug.Log("Player dealt " + attackDamage + " damage to Enemy: " + other.name);
                    return;
                }
            }
        }
    }

}