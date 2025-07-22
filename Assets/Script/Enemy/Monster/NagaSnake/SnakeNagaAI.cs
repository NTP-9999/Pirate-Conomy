#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
#endif

public class SnakeNagaAI : MonoBehaviour
{
    [HideInInspector] public NagaStateMachine StateMachine;
    [HideInInspector] public NagaPatrolState patrolState;
    [HideInInspector] public NagaChaseState chaseState;
    [HideInInspector] public NagaAttackState attackState;

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Player;  // link via Inspector หรือหาโดย Tag

    [Header("Ranges")]
    public float patrolRadius = 10f;
    public float patrolPointReachThreshold = 1f;
    public float chaseRange = 15f;
    public float attackRange = 3f;
    public float timeBetweenAttacks = 2f;
    [HideInInspector] public Vector3 patrolCenter;
    [Header("Attack Damage")]
    public float[] attackDamages = new float[3] { 10f, 15f, 20f };


    public Vector3 Position => transform.position;
    public Vector3 PlayerPosition => Player.position;

    void Awake()
    {
        StateMachine = new NagaStateMachine();
        patrolState = new NagaPatrolState(this);
        chaseState = new NagaChaseState(this);
        attackState = new NagaAttackState(this);

        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;

        patrolCenter = transform.position;
    }

    void Start()
    {
        StateMachine.Initialize(patrolState);
    }

    void Update()
    {
        StateMachine.Update();
        Animator.SetFloat("Speed", Agent.velocity.magnitude);
    }

    public bool IsPlayerInChaseRange()
    {
        return Vector3.Distance(Position, PlayerPosition) <= chaseRange;
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(Position, PlayerPosition) <= attackRange;
    }

    public void RotateTowardsPlayer()
    {
        var dir = (PlayerPosition - Position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
    void OnDrawGizmosSelected()
    {
        // ตำแหน่งต้นกำเนิด
        Vector3 center = Application.isPlaying ? patrolCenter : transform.position;

        // Patrol area (คงที่)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, patrolRadius);

        // Chase & Attack (ติดตามงู)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void DealDamage(int atkIndex)
    {
        // ตรวจระยะอีกทีให้แน่ใจ
        if (!IsPlayerInAttackRange()) return;

        // ดึงสคริปต์ CharacterStats จาก Player แล้วเรียก TakeDamage
        var stats = Player.GetComponent<CharacterStats>();
        if (stats != null)
        {
            // atkIndex-1 เพราะ array เริ่มที่ 0
            float dmg = attackDamages[Mathf.Clamp(atkIndex-1, 0, attackDamages.Length-1)];
            stats.TakeDamage(dmg);
        }
    }
}
