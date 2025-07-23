// CrabAI.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrabAI : LivingThing
{
    [HideInInspector] public CrabStateMachine StateMachine;
    [HideInInspector] public CrabHideState    hideState;
    [HideInInspector] public CrabChaseState   chaseState;
    [HideInInspector] public CrabAttackState  attackState;
    [HideInInspector] public CrabHurtState    hurtState;

    public NavMeshAgent Agent;
    public Animator      Animator;
    public Transform     Player;

    [Header("Ranges")]
    public float patrolRadius;                // ถ้าไม่ใช้ก็เว้นไว้
    public float chaseRange      = 15f;
    public float attackRange     = 3f;
    public float timeBetweenAttacks = 2f;

    [Header("Attack Damage")]
    public float[] attackDamages = new float[3] { 10f, 15f, 20f };

    [Header("Hurt Settings")]
    public float hurtDuration = 0.5f;

    public Vector3 Position       => transform.position;
    public Vector3 PlayerPosition => Player.position;

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new CrabStateMachine();
        hideState    = new CrabHideState(this);
        chaseState   = new CrabChaseState(this);
        attackState  = new CrabAttackState(this);
        hurtState    = new CrabHurtState(this);

        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Start()
    {
        StateMachine.Initialize(hideState);
    }

    void Update()
    {
        StateMachine.Update();
        Animator.SetFloat("Speed", Agent.velocity.magnitude);
    }

    public bool IsPlayerInChaseRange()
        => Vector3.Distance(Position, PlayerPosition) <= chaseRange;

    public bool IsPlayerInAttackRange()
        => Vector3.Distance(Position, PlayerPosition) <= attackRange;

    public void RotateTowardsPlayer()
    {
        var dir = (PlayerPosition - Position).normalized;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
