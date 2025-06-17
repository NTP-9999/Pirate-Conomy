using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Reference to the Animator component on this GameObject.")]
    [SerializeField] private Animator playerAnimator;
    [Tooltip("Reference to the PlayerWeaponManager script on this GameObject.")]
    [SerializeField] private PlayerWeaponManager playerWeaponManager;
    [Tooltip("Reference to the BasicCombat script on this GameObject.")]
    [SerializeField] private BasicCombat basicCombat; // เพื่อใช้ Cooldown และ Input

    [Header("Attack Settings")]
    [Tooltip("The tag assigned to your melee weapon's GameObject (e.g., 'PlayerMeleeWeapon').")]
    [SerializeField] private string meleeWeaponTag = "PlayerMeleeWeapon"; // อาจจะต้องใช้ Tag เพื่อหา PlayerAttackCollider
    [Tooltip("The tag assigned to your ranged weapon's GameObject (e.g., 'PlayerRangedWeapon').")]
    [SerializeField] private string rangedWeaponTag = "PlayerRangedWeapon"; // อาจจะต้องใช้ Tag

    // Events ที่ Animation จะเรียก (เราจะกำหนดใน Animation Clip)
    // การใช้ Event แบบนี้จะแม่นยำกว่าการใช้ StateMachineBehaviour มาก
    public void EnableMeleeHitboxEvent()
    {
        if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.GetCurrentWeaponType() == WeaponType.Melee)
        {
            MeleeWeapon meleeWeapon = playerWeaponManager.CurrentWeapon as MeleeWeapon;
            if (meleeWeapon != null)
            {
                PlayerAttackCollider attackCollider = meleeWeapon.GetComponent<PlayerAttackCollider>();
                if (attackCollider != null)
                {
                    attackCollider.EnableHitbox();
                    Debug.Log("Animation Event: Melee Hitbox Enabled!");
                }
            }
        }
    }

    public void DisableMeleeHitboxEvent()
    {
        if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.GetCurrentWeaponType() == WeaponType.Melee)
        {
            MeleeWeapon meleeWeapon = playerWeaponManager.CurrentWeapon as MeleeWeapon;
            if (meleeWeapon != null)
            {
                PlayerAttackCollider attackCollider = meleeWeapon.GetComponent<PlayerAttackCollider>();
                if (attackCollider != null)
                {
                    attackCollider.DisableHitbox();
                    Debug.Log("Animation Event: Melee Hitbox Disabled!");
                }
            }
        }
    }

    public void PerformRangedShotEvent()
    {
        if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.GetCurrentWeaponType() == WeaponType.Ranged)
        {
            RangedWeapon rangedWeapon = playerWeaponManager.CurrentWeapon as RangedWeapon;
            if (rangedWeapon != null)
            {
                rangedWeapon.PerformRangedAttack(); // เรียกเมธอดการยิงของปืน
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
        if (Input.GetMouseButtonDown(1) && basicCombat.CanAttack) // BasicCombat จะจัดการ CanAttack()
        {
            // Trigger Cooldown ใน BasicCombat
            basicCombat.TriggerAttackCooldown();

            // สั่งแอนิเมชันตามอาวุธที่ถืออยู่
            WeaponType currentWeapon = playerWeaponManager.GetCurrentWeaponType();
            if (currentWeapon == WeaponType.Melee)
            {
                playerAnimator.SetTrigger("AttackMelee");
                Debug.Log("Player triggered Melee Attack Animation.");
            }
            else if (currentWeapon == WeaponType.Ranged)
            {
                playerAnimator.SetTrigger("AttackRanged");
                Debug.Log("Player triggered Ranged Attack Animation.");
            }
        }
    }
}