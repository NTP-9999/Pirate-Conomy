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
        Vector3 pos = transform.position;

        // Patrol Radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, patrolRadius);

        // Chase Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, chaseRange);

        // Attack Range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
