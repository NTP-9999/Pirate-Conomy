using UnityEngine;

public class KravalonAI : MonoBehaviour
{
    [HideInInspector] public KravalonStateMachine StateMachine;
    [HideInInspector] public KravalonChaseState chaseState;
    [HideInInspector] public KravalonAttackIdleState attackIdleState;
    [HideInInspector] public KravalonAttackState attackState;
    [HideInInspector] public KravalonHurtState hurtState;

    [Header("Health")]
    public float maxHealth = 200f;
    private float currentHealth;

    public Transform shipTarget;
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 3f;
    public float idleBeforeAttack = 0.8f;
    public float damage = 40f;

    private float lastAttackTime = -Mathf.Infinity;

    void Awake()
    {
        StateMachine = new KravalonStateMachine();
        chaseState = new KravalonChaseState(this);
        attackIdleState = new KravalonAttackIdleState(this);
        attackState = new KravalonAttackState(this);
        hurtState = new KravalonHurtState(this);
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (shipTarget == null && ShipEnterExit.Instance != null)
            shipTarget = ShipEnterExit.Instance.transform;

        StateMachine.Initialize(chaseState);
    }

    void Update()
    {
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

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StateMachine.ChangeState(hurtState);
        }
    }

    void Die()
    {
        Debug.Log("💀 Kravalon ตายแล้ว!");

        // TODO: เล่น animation ตาย, จม, ฯลฯ
        Destroy(gameObject, 3f); // รอให้แอนิเมชันจบก่อนลบ
    }
}
