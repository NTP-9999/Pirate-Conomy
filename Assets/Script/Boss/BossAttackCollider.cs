using UnityEngine;

public class BossAttackCollider : MonoBehaviour
{
    public float damageAmount = 10f;
    private Collider attackCollider;
    private bool hasDealtDamageThisAttack = false; // ตัวแปรนี้ต้องอยู่ในคลาส BossAttackCollider เท่านั้น

    void Start()
    {
        attackCollider = GetComponent<Collider>();
        if (attackCollider == null)
        {
            Debug.LogError("BossAttackCollider: Collider component not found! This script requires a Collider.");
            enabled = false;
            return;
        }
        if (!attackCollider.isTrigger)
        {
            Debug.LogWarning("BossAttackCollider: Collider is not set to Is Trigger. Setting it now.");
            attackCollider.isTrigger = true;
        }
        attackCollider.enabled = false;
        Debug.Log("BossAttackCollider initialized. Hitbox is OFF.");
    }

    public void EnableHitbox()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            hasDealtDamageThisAttack = false; // รีเซ็ตสถานะการสร้างดาเมจเมื่อเปิด Hitbox ใหม่
            Debug.Log("Hitbox Enabled.");
        }
    }

    public void DisableHitbox()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
            Debug.Log("Hitbox Disabled.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"Boss attack trigger entered by: {other.gameObject.name}. Has dealt damage: {hasDealtDamageThisAttack}. Layer: {other.gameObject.layer}");

        if (hasDealtDamageThisAttack) return; // ถ้าตีไปแล้วในการโจมตีครั้งนี้ ก็ไม่ต้องทำอะไร

        // ตรวจสอบ Tag ของผู้เล่น
        if (other.CompareTag("Player"))
        {
            // ***** แก้ไขตรงนี้: เปลี่ยนจาก PlayerHealth เป็น CharacterStats *****
            CharacterStats characterStats = other.GetComponent<CharacterStats>(); 
            if (characterStats != null) 
            {
                characterStats.TakeDamage(damageAmount); // เรียก TakeDamage ของ CharacterStats
                Debug.Log($"Player hit! Player took {damageAmount} damage. Remaining health: {characterStats.currentHealth}");
                hasDealtDamageThisAttack = true; // ตั้งค่าว่าสร้างดาเมจแล้วในการโจมตีนี้
            }
            else
            {
                Debug.LogWarning("BossAttackCollider: Player detected, but CharacterStats script not found on " + other.gameObject.name);
            }
        }
    }

    // เมธอดสำหรับรีเซ็ตสถานะการทำดาเมจ (จะถูกเรียกจาก BossAI ผ่าน Animation Event)
    public void ResetDamageStatus()
    {
        hasDealtDamageThisAttack = false; // ใช้ตัวแปรชื่อที่ถูกต้อง
        Debug.Log("BossAttackCollider: Damage status reset.");
    }
}