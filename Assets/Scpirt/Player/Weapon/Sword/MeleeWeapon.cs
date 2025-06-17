using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Weapon Settings")]
    [Tooltip("Reference to the PlayerAttackCollider component on this weapon's GameObject.")]
    [SerializeField] private PlayerAttackCollider playerAttackCollider;

    void Awake()
    {
        // ตรวจสอบให้แน่ใจว่าได้ตั้งค่า WeaponType เป็น Melee
        if (Type != WeaponType.Melee)
        {
            Debug.LogWarning($"MeleeWeapon script on {gameObject.name} has WeaponType set to {Type}. It should be WeaponType.Melee.", this);
        }

        // พยายามหา PlayerAttackCollider หากยังไม่ได้ลากใส่ใน Inspector
        if (playerAttackCollider == null)
        {
            playerAttackCollider = GetComponent<PlayerAttackCollider>();
            if (playerAttackCollider == null)
            {
                Debug.LogError($"MeleeWeapon: PlayerAttackCollider not found on {gameObject.name}! Melee attacks will not register damage.", this);
            }
        }
    }

    // อาจจะมีเมธอดสำหรับสั่งโจมตี (เรียกจาก BasicCombat)
    public void PerformMeleeAttack()
    {
        // ในเกมของคุณ Hitbox ถูกควบคุมโดย PlayerAttackStateBehaviour
        // ดังนั้นตรงนี้อาจจะไม่ต้องทำอะไรมาก นอกจาก Debug.Log เพื่อยืนยันว่าเรียกถูก
        // หรืออาจจะใช้เพื่อ Play Sound effect เฉพาะของดาบ
        Debug.Log($"{WeaponName} is performing a melee attack!");
        // ถ้าต้องการเปิด/ปิด hitbox จากตรงนี้โดยตรง (แทนที่จะเป็น StateMachineBehaviour)
        // playerAttackCollider.EnableHitbox();
        // (แต่ปัจจุบันใช้ PlayerAttackStateBehaviour ซึ่งดีแล้ว)
    }

    // เราจะ Override Equip และ Unequip เพื่อควบคุม hitbox ของดาบโดยตรง
    public override void Equip()
    {
        base.Equip(); // เรียก Equip ของคลาสแม่ (เปิด GameObject)
        // เมื่อถือดาบ ให้แน่ใจว่า hitbox ปิดอยู่ก่อนเริ่ม attack state
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
            playerAttackCollider.ResetDamageDealt(); // รีเซ็ตสถานะดาเมจเสมอเมื่อ equip
        }
    }

    public override void Unequip()
    {
        base.Unequip(); // เรียก Unequip ของคลาสแม่ (ปิด GameObject)
        // เมื่อเก็บดาบ ให้แน่ใจว่า hitbox ปิดอยู่
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
        }
    }
}