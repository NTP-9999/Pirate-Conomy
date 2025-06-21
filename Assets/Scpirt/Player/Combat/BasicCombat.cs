using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Reference to the Player Animator Controller.")]
    [SerializeField] public Animator playerAnimator; // ทำให้เป็น public เพื่อให้อาวุธอื่นเข้าถึงได้

    [Header("Combat Settings")]
    [SerializeField] private float globalAttackCooldown = 0.5f; // Cooldown ทั่วไปของการโจมตี
    private float nextGlobalAttackTime = 0f;
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
    }

    void Start()
    {
       
    }
    public void TriggerAttackCooldown()
    {
        nextGlobalAttackTime = Time.time + globalAttackCooldown;
        Debug.Log("BasicCombat: Global attack cooldown triggered.");
    }
    public void OnAttackAnimationEnd()
    {
        IsAttacking = false;
        Debug.Log("BasicCombat: Attack animation ended. IsAttacking is now false.");
    }
}