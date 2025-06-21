using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private LivingEntityStats selfStats;
    private float lastAttackTime = -999f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        selfStats = GetComponent<LivingEntityStats>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (selfStats.IsDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            // เดิน
            animator.SetBool("isWalking", distance > attackRange);

            // โจมตี
            if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        animator.SetTrigger("Attack");

        // แก้จาก LivingEntityStats เป็น CharacterStats
        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("MonsterAI: Player does not have CharacterStats component!");
        }
    }

}
