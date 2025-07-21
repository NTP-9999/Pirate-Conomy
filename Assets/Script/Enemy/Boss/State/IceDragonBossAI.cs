using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(BossStat), typeof(NavMeshAgent))]
public class IceDragonBossAI : MonoBehaviour
{
    [Header("Detection & Cooldowns")]
    public float detectionRange = 15f;
    public float attackCooldown = 2f;

    [Header("Damage Settings")]
    public float tailRadius = 2f;
    public float tailDamage = 15f;
    public float scratchRadius = 4f;
    public float scratchDamage = 25f;
    [Header("Attack Settings")]
    public float attackRange = 25f;

    [HideInInspector] public Transform player;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public BossStateMachine stateMachine;
    private float cooldownTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = new BossStateMachine();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cooldownTimer = 0f;

        // NavMeshAgent tuning:
        agent.speed = 8f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = attackRange;
    }

    private void Start()
    {
        stateMachine.ChangeState(new BossIdleState(this));
    }

    private void Update()
    {
        cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
        stateMachine.Tick();
    }

    public bool CanAttack()
        => cooldownTimer <= 0f;

    public void ResetAttackCooldown()
        => cooldownTimer = attackCooldown;

    public void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, look, Time.deltaTime * 5f
            );
        }
    }

    public void OnTailSwipeHit()
    {
        var hits = Physics.OverlapSphere(transform.position, tailRadius);
        foreach (var hit in hits)
            if (hit.CompareTag("Player"))
                hit.GetComponent<CharacterStats>()?.TakeDamage(tailDamage);
    }

    public void OnScratchHit()
    {
        var hits = Physics.OverlapSphere(transform.position, scratchRadius);
        foreach (var hit in hits)
            if (hit.CompareTag("Player"))
                hit.GetComponent<CharacterStats>()?.TakeDamage(scratchDamage);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
    }
}
