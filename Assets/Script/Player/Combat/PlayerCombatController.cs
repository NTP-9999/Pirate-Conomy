using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Reference to the Animator component on this GameObject.")]
    [SerializeField] private Animator playerAnimator;
    [Tooltip("Reference to the PlayerWeaponManager script on this GameObject.")]
    [SerializeField] private PlayerWeaponManager playerWeaponManager; // ต้องมี PlayerWeaponManager ใน Hierarchy
    [Tooltip("Reference to the BasicCombat script on this GameObject.")]
    [SerializeField] private BasicCombat basicCombat; // เพื่อใช้ Cooldown และ Input

    [Header("Attack Settings")]
    [Tooltip("The tag assigned to your melee weapon's GameObject (e.g., 'PlayerMeleeWeapon').")]
    [SerializeField] private string meleeWeaponTag = "PlayerMeleeWeapon"; // อาจจะต้องใช้ Tag เพื่อหา PlayerAttackCollider
    [Tooltip("The tag assigned to your ranged weapon's GameObject (e.g., 'PlayerRangedWeapon').")]
    [SerializeField] private string rangedWeaponTag = "PlayerRangedWeapon"; // อาจจะต้องใช้ Tag

    // Events ที่ Animation จะเรียก (เราจะกำหนดใน Animation Clip)
    // การใช้ Event แบบนี้จะแม่นยำกว่าการใช้ StateMachineBehaviour มาก

    // **เมธอดสำหรับ Melee Weapon Events (จาก Animation Event)**
    public void EnableMeleeHitboxEvent()
    {
        Debug.Log(">> Animation Event: EnableMeleeHitboxEvent Called!");
        if (playerWeaponManager.CurrentWeapon is MeleeWeapon melee)
        {
            melee.PerformMeleeAttackEvent();
        }
    }

    public void DisableMeleeHitboxEvent()
    {
        Debug.Log(">> Animation Event: DisableMeleeHitboxEvent Called!");
        if (playerWeaponManager.CurrentWeapon is MeleeWeapon melee)
        {
            melee.StopMeleeAttackEvent();
        }
    }


    // **เมธอดสำหรับ Ranged Weapon Events (จาก Animation Event)**
    public void PerformRangedShotEvent() // นี่คือเมธอดที่ Animation Event จะเรียก
    {
        if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.GetCurrentWeaponType() == WeaponType.Ranged)
        {
            RangedWeapon rangedWeapon = playerWeaponManager.CurrentWeapon as RangedWeapon;
            if (rangedWeapon != null)
            {
                rangedWeapon.PerformRangedAttackEvent(); // **แก้ไขตรงนี้: เรียก PerformRangedAttackEvent**
                Debug.Log("Animation Event: Ranged Shot Fired!");
            }
        }
    }

    void Awake()
    {
        // ตรวจสอบ Reference ให้แน่ใจ
        if (playerAnimator == null) playerAnimator = GetComponent<Animator>();
        if (playerWeaponManager == null) playerWeaponManager = GetComponent<PlayerWeaponManager>();
        if (basicCombat == null) basicCombat = GetComponent<BasicCombat>();

        if (playerAnimator == null || playerWeaponManager == null || basicCombat == null)
        {
            Debug.LogError("PlayerCombatController: Missing essential components on " + gameObject.name + "!", this);
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Logic การโจมตี: ใช้ BasicCombat เพื่อจัดการ Input และ Cooldown
        // **เราควรใช้ GetMouseButtonDown(0) สำหรับการโจมตีหลัก (คลิกซ้าย) ตามที่คุณต้องการ**
        if (Input.GetMouseButtonDown(0) && basicCombat.CanAttack) // คลิกซ้าย และไม่ติด Cooldown
        {
            // Trigger Cooldown ใน BasicCombat
            basicCombat.TriggerAttackCooldown();

            // สั่งแอนิเมชันตามอาวุธที่ถืออยู่
            WeaponType currentWeaponType = playerWeaponManager.GetCurrentWeaponType(); // เปลี่ยนชื่อตัวแปรเพื่อไม่ให้ซ้ำกับ currentWeapon

            // ตรงนี้คือจุดที่ PlayerCombatController นี้มีหน้าที่สั่ง Animator โดยตรง
            // และให้ BasicCombat ทำหน้าที่จัดการ Cooldown เท่านั้น
            if (currentWeaponType == WeaponType.Melee)
            {
                playerAnimator.SetTrigger("AttackMelee");
                Debug.Log("Player triggered Melee Attack Animation.");
            }
            else if (currentWeaponType == WeaponType.Ranged)
            {
                playerAnimator.SetTrigger("AttackRanged");
                Debug.Log("Player triggered Ranged Attack Animation.");
            }
            else
            {
                Debug.LogWarning("PlayerCombatController: No known weapon type equipped for attack.");
            }
        }
    }
}