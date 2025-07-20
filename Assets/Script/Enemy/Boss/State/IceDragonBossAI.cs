using UnityEngine;

[RequireComponent(typeof(Animator), typeof(BossStat))]
public class IceDragonBossAI : MonoBehaviour
{
    [Header("Detection & Cooldowns")]
    public float detectionRange = 15f;
    public float attackCooldown = 2f;

    private Transform player;
    [HideInInspector] public Animator animator;
    [HideInInspector] public BossStateMachine stateMachine;
    private float cooldownTimer;

    private void Awake()
    {
        animator      = GetComponent<Animator>();
        stateMachine  = new BossStateMachine();
        player        = GameObject.FindGameObjectWithTag("Player").transform;
        cooldownTimer = 0f;
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

    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    public bool CanAttack()
    {
        return cooldownTimer <= 0f;
    }

    public void ResetAttackCooldown()
    {
        cooldownTimer = attackCooldown;
    }

    public void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }
    }
}
