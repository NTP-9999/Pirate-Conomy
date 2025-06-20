using UnityEngine;

public class MeleeWeapon : Weapon // ตรวจสอบว่าสืบทอดจาก Weapon
{
    [Header("Melee Weapon Settings")]
    [Tooltip("Reference to the PlayerAttackCollider component on this weapon's GameObject.")]
    [SerializeField] private PlayerAttackCollider playerAttackCollider;

    void Awake() // เปลี่ยนเป็น protected override void Awake() ถ้า Weapon มี Awake() ด้วย
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

    // เพิ่ม override Attack() เพื่อให้ PlayerWeaponManager เรียกใช้ได้
    public override void Attack()
    {
        // ในโครงสร้างนี้ PlayerCombatController จะสั่ง Animator โดยตรง
        // ดังนั้น MeleeWeapon.Attack() ไม่จำเป็นต้องสั่ง Animator อีก
        // แต่คุณสามารถใช้ตรงนี้เพื่อเล่น Sound effect เฉพาะของดาบ หรืออื่นๆ ได้
        Debug.Log($"{WeaponName} Attack() called. Ready for animation events.");
    }

    // เมธอดสำหรับสั่งโจมตี (จะถูกเรียกจาก Animation Event)
    public void PerformMeleeAttackEvent()
    {
        Debug.Log($"[MeleeWeapon] Enable Hitbox: {WeaponName}");
        if (playerAttackCollider != null)
        {
            playerAttackCollider.EnableHitbox();
        }
    }

    public void StopMeleeAttackEvent()
    {
        Debug.Log($"[MeleeWeapon] Disable Hitbox: {WeaponName}");
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
            playerAttackCollider.ResetDamageDealt();
        }
    }


    public override void Equip()
    {
        base.Equip();
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
            playerAttackCollider.ResetDamageDealt();
        }
    }

    public override void Unequip()
    {
        base.Unequip();
        if (playerAttackCollider != null)
        {
            playerAttackCollider.DisableHitbox();
        }
    }
}