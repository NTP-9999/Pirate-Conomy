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
            hasDealtDamageThisAttackInstance = false; // รีเซ็ตเมื่อ hitbox ถูกเปิด
            // Debug.Log(gameObject.name + " Player Hitbox Enabled.");
        }
    }

    public void DisableHitbox()
    {
        if (playerWeaponCollider != null)
        {
            playerWeaponCollider.enabled = false;
            // Debug.Log(gameObject.name + " Player Hitbox Disabled.");
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
            if (other.CompareTag("Boss"))
            {
                BossStats bossStats = other.GetComponent<BossStats>();
                if (bossStats != null)
                {
                    bossStats.TakeDamage(attackDamage);
                    hasDealtDamageThisAttackInstance = true; 
                    Debug.Log("Player dealt " + attackDamage + " damage to boss!");
                }
            }
        }
    }
}