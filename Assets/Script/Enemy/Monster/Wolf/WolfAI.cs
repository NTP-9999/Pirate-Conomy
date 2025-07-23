// WolfAI.cs
using UnityEngine;
using UnityEngine.AI;

public class WolfAI : LivingThing
{
    [HideInInspector] public WolfStateMachine StateMachine;
    [HideInInspector] public WolfPatrolState patrolState;
    [HideInInspector] public WolfChaseState chaseState;
    [HideInInspector] public WolfAttackState attackState;
    [HideInInspector] public WolfHurtState hurtState;

    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform Player;

    [Header("Ranges")]
    public float patrolRadius = 10f;
    public float patrolPointReachThreshold = 1f;
    public float chaseRange = 15f;
    public float attackRange = 3f;
    public float timeBetweenAttacks = 2f;
    [HideInInspector] public Vector3 patrolCenter;

    [Header("Attack Damage")]
    public float[] attackDamages = new float[3] { 10f, 15f, 20f };

    [Header("Hurt Settings")]
    public float hurtDuration = 0.5f;

    public Vector3 Position => transform.position;
    public Vector3 PlayerPosition => Player.position;

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new WolfStateMachine();
        patrolState = new WolfPatrolState(this);
        chaseState = new WolfChaseState(this);
        attackState = new WolfAttackState(this);
        hurtState = new WolfHurtState(this);

        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        patrolCenter = transform.position;
    }

    protected override void Start()
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
        Vector3 dir = (PlayerPosition - Position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void DealDamage(int atkIndex)
    {
        if (!IsPlayerInAttackRange()) return;

        var stats = Player.GetComponent<CharacterStats>();
        if (stats != null)
        {
            float dmg = attackDamages[Mathf.Clamp(atkIndex - 1, 0, attackDamages.Length - 1)];
            stats.TakeDamage(dmg);
        }
    }

    protected override void OnDamageTaken(float damage)
    {
        base.OnDamageTaken(damage);
        if (!IsDead)
            StateMachine.ChangeState(hurtState);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? patrolCenter : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif

    public override void OnDeath()
    {
        Agent.isStopped = true;
        Animator.SetTrigger("Die");
        Destroy(gameObject, 2f); // Destroy after 2 seconds to allow death animation to play
    }
}
