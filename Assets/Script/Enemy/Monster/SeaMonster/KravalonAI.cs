using UnityEngine;

public class KravalonAI : MonoBehaviour
{
    [HideInInspector] public KravalonStateMachine StateMachine;
    [HideInInspector] public KravalonChaseState chaseState;
    [HideInInspector] public KravalonAttackIdleState attackIdleState;
    [HideInInspector] public KravalonAttackState attackState;

    [Header("Health")]
    public float maxHealth = 200f;
    private float currentHealth;
    public Animator animator;

    public Transform shipTarget;
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 3f;
    public float idleBeforeAttack = 0.8f;
    public float damage = 40f;
    public float stoppingDistance = 3f;
    public float rotationSpeed = 5f;

    private float lastAttackTime = -Mathf.Infinity;

    void Awake()
    {
         StateMachine       = new KravalonStateMachine();
        chaseState         = new KravalonChaseState(this);
        attackState        = new KravalonAttackState(this);
        currentHealth      = maxHealth;
    }

    void Start()
    {
        if (shipTarget == null && ShipEnterExit.Instance != null)
        shipTarget = ShipEnterExit.Instance.transform;

    // เริ่มจากไล่ก่อน
        StateMachine.Initialize(chaseState);
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        RotateTowardsShip(Time.deltaTime);      // ← เพิ่มบรรทัดนี้
        StateMachine.Update();
    }

    public bool IsShipInAttackRange()
    {
        return Vector3.Distance(transform.position, shipTarget.position) <= attackRange;
    }

    public bool IsAttackCooldownReady()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }
    public bool IsShipInStoppingDistance()
{
        return Vector3.Distance(transform.position, shipTarget.position) <= stoppingDistance;
    }
    /// <summary>
    /// หมุน Kravalon ให้หันหน้าไปหาเรือ โดยล็อคแกน Y
    /// ทำงานทุกเฟรมก่อน State Logic
    /// </summary>
    public void RotateTowardsShip(float deltaTime)
    {
        if (shipTarget == null) return;
        // 1) หาทิศทาง
        Vector3 dir = shipTarget.position - transform.position;
        // 2) ล็อค Y
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        // 3) คำนวณ rotation เป้าหมาย
        Quaternion targetRot = Quaternion.LookRotation(dir);
        // 4) ค่อย ๆ หมุน
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            deltaTime * rotationSpeed     // rotationSpeed กำหนดใน KravalonAI
        );
    }
    public void SetAttackCooldown()
    {
        lastAttackTime = Time.time;
    }

    public void DealDamageToShip()
    {
        if (shipTarget.TryGetComponent<ShipStats>(out var stats))
        {
            stats.TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Kravalon โดนยิง! HP: {currentHealth}");
    }

    void Die()
    {
        animator.SetTrigger("Die");
        Debug.Log("💀 Kravalon ตายแล้ว!");
        Destroy(gameObject, 2f);
    }
}
