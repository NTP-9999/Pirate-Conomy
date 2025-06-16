using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Combat Audio")]
    public AudioSource playerAudioSource;
    public AudioClip attackSwingSFX;
    public AudioClip bossHitSFX;

    [Header("Attack Settings")]
    public float attackRate = 1f;
    public float staminaCost = 20f;
    public float attackDamage = 30f;
    public float attackRange = 2f;
    public LayerMask enemyLayer;

    [HideInInspector] public bool isAttacking = false;

    private float nextAttackTime = 0f;
    private Animator animator;
    private CharacterStats stats;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();

        if (playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>();
            if (playerAudioSource == null)
            {
                Debug.LogWarning("BasicCombat: AudioSource component not found on this GameObject. Combat SFX will not play.", this);
            }
        }
    }

    void Update()
    {
        if (stats == null || stats.isDead) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isAttacking)
        {
            if (stats.currentStamina >= staminaCost)
            {
                Attack();
                stats.UseStamina(staminaCost);
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else
            {
                Debug.Log("Not enough stamina for attack!");
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        if (playerAudioSource != null && attackSwingSFX != null)
        {
            playerAudioSource.PlayOneShot(attackSwingSFX);
            Debug.Log("Played SFX: Attack Swing!");
        }
    }

    // เมธอดนี้จะถูกเรียกโดย Animation Event
    public void CheckForHit()
    {
        Debug.Log("CheckForHit method called by Animation Event!");

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, attackRange, enemyLayer))
        {
            Debug.Log("Raycast hit something: " + hit.collider.name + " on Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            ApplyDamageToTarget(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("Attack missed. Raycast hit nothing within range " + attackRange + " on layer " + LayerMask.LayerToName(enemyLayer.value));
        }
    }

    public void ApplyDamageToTarget(GameObject target)
    {
        Debug.Log("ApplyDamageToTarget called for: " + target.name);

        if (target == null) return;

        // *** การเปลี่ยนแปลงสำคัญตรงนี้: ใช้ GetComponentInParent แทน GetComponent ***
        // นี่คือจุดที่จะหา BossStats จาก GameObject ที่ถูกชน หรือจาก Parent ของมัน
        BossStats boss = target.GetComponentInParent<BossStats>();

        // ตรวจสอบ CharacterStats (ส่วนนี้ยังคงเหมือนเดิม)
        CharacterStats targetStats = target.GetComponent<CharacterStats>();
        if (targetStats == null) // ถ้า GameObject ที่โดนชนไม่มี CharacterStats
        {
            // อาจจะลองหา CharacterStats ใน Parent ด้วยก็ได้ ถ้าโครงสร้างซับซ้อน
            targetStats = target.GetComponentInParent<CharacterStats>();
            if (targetStats == null)
            {
                Debug.Log("Target or its parent does NOT have CharacterStats. Cannot apply damage.");
                return; // ออกจากฟังก์ชันถ้าหา CharacterStats ไม่เจอ
            }
            Debug.Log("CharacterStats found on parent: " + targetStats.gameObject.name);
        }
        else
        {
            Debug.Log("Target has CharacterStats on itself.");
        }
        if (boss != null)
        {
            Debug.Log("CONFIRMED: Target is Boss! BossStats found on: " + boss.gameObject.name);

            // *** เพิ่ม Debug Log ตรวจสอบ AudioSource และ AudioClip ***
            if (playerAudioSource == null)
            {
                Debug.LogError("Error: playerAudioSource is NULL! Cannot play Boss Hit SFX.");
            }
            if (bossHitSFX == null)
            {
                Debug.LogError("Error: bossHitSFX AudioClip is NULL! Cannot play Boss Hit SFX.");
            }

            if (playerAudioSource != null && bossHitSFX != null)
            {
                playerAudioSource.PlayOneShot(bossHitSFX);
                Debug.Log("Played SFX: Player hit Boss!");
            }
            else
            {
                Debug.LogWarning("Boss Hit SFX or Player AudioSource is NULL. Audio not playing."); // อันนี้จะขึ้นเมื่อตัวใดตัวหนึ่งเป็น null
            }

            targetStats.TakeDamage(attackDamage);
            Debug.Log(target.name + " (Boss) took " + attackDamage + " damage. Current HP: " + targetStats.currentHealth);
        }
        else
        {
            Debug.Log("FAILURE: BossStats was NOT found on target or any of its parents. This is a regular enemy.");
            targetStats.TakeDamage(attackDamage);
            Debug.Log(target.name + " took " + attackDamage + " damage. Current HP: " + targetStats.currentHealth);
        }
        Debug.Log("--- End ApplyDamageToTarget ---");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * attackRange);
    }
}