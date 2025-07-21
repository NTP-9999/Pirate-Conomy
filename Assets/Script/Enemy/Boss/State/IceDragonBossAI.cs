using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(BossStat), typeof(NavMeshAgent))]
public class IceDragonBossAI : MonoBehaviour
{
    [Header("Detection & Cooldowns")]
    public float detectionRange = 15f;
    public float attackCooldown = 2f;
    [Header("Attack Settings")]
    public float attackRange = 25f;

    [Header("HitBoxes")]
    public HitBox scratchHitBoxL;
    public HitBox scratchHitBoxR;

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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
    }
    public void EnableScratchL()
    {
        scratchHitBoxL.SetActive(true);
    }

    public void DisableScratchL()
    {
        scratchHitBoxL.SetActive(false);
        scratchHitBoxL.alreadyHit = false;
    }

    public void EnableScratchR()
    {
        scratchHitBoxR.SetActive(true);
    }

    public void DisableScratchR()
    {
        scratchHitBoxR.SetActive(false);
        scratchHitBoxR.alreadyHit = false;
    }
}
