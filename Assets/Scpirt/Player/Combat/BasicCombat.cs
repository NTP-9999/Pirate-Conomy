using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Reference to the Player Animator Controller.")]
    [SerializeField] public Animator playerAnimator; // ทำให้เป็น public เพื่อให้อาวุธอื่นเข้าถึงได้

    [Header("Combat Settings")]
    [SerializeField] private float globalAttackCooldown = 0.5f; // Cooldown ทั่วไปของการโจมตี
    private float nextGlobalAttackTime = 0f;

    // ไม่มี Weapon Management ตรงนี้แล้ว เพราะ PlayerWeaponManager จัดการ
    // [Header("Weapon Management")]
    // [Tooltip("The current weapon the player is holding. Assign in Inspector.")]
    // [SerializeField] private Weapon currentWeapon; 
    // [Tooltip("The parent transform where weapons will be equipped.")]
    // [SerializeField] private Transform weaponHolder; 

    public bool IsAttacking { get; private set; } = false; // สถานะว่ากำลังโจมตีอยู่หรือไม่

    // Property เพื่อให้ PlayerCombatController เช็ค Cooldown ได้
    public bool CanAttack
    {
        get { return Time.time >= nextGlobalAttackTime; }
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
        // ไม่ต้องเช็ค weaponHolder ตรงนี้แล้ว
    }

    void Start()
    {
        // BasicCombat จะไม่จัดการ Equip อาวุธเริ่มต้นแล้ว PlayerWeaponManager จะจัดการแทน
    }

    // เมธอดสำหรับสั่งให้เริ่ม Cooldown การโจมตี (เรียกจาก PlayerCombatController)
    public void TriggerAttackCooldown()
    {
        nextGlobalAttackTime = Time.time + globalAttackCooldown;
        Debug.Log("BasicCombat: Global attack cooldown triggered.");
    }

    // เมธอดที่ถูกเรียกจาก Animation Event เมื่อ Animation โจมตีจบ (ถ้ายังต้องการใช้)
    // หรือคุณอาจจะให้ PlayerCombatController รับผิดชอบ OnAttackAnimationEnd โดยตรง
    // เนื่องจาก PlayerCombatController เป็นตัวสั่ง Animator
    public void OnAttackAnimationEnd()
    {
        IsAttacking = false;
        Debug.Log("BasicCombat: Attack animation ended. IsAttacking is now false.");
    }

    // เมธอดสำหรับรับ Animator จากภายนอก (เผื่อเรียกจาก Weapon หรือ script อื่นๆ)
    // แต่จริงๆ แล้ว Weapon จะได้รับ playerAnimator โดยตรงจาก BasicCombat.playerAnimator
    // public Animator GetPlayerAnimator()
    // {
    //     return playerAnimator;
    // }
}