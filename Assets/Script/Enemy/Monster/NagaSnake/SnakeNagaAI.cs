using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SnakeNagaAI : LivingThing
{
    [HideInInspector] public NagaStateMachine StateMachine;
    [HideInInspector] public NagaPatrolState patrolState;
    [HideInInspector] public NagaChaseState chaseState;
    [HideInInspector] public NagaAttackState attackState;
    [HideInInspector] public NagaHurtState hurtState;
    [HideInInspector] public NagaPoisonState poisonState;
    [HideInInspector] public NagaMeteorState meteorState;
    [HideInInspector] public NagaAttackIdleState attackIdleState;


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

    [Header("Hurt Settings")]
    public float hurtDuration = 0.5f;
    [Header("Poison Skill")]
    public GameObject poisonVFXPrefab;        // Prefab พิษ
    public Transform poisonSpawnPoint;       // ตำแหน่งยิงพิษ
    public float poisonSpawnDelay = 0.6f;  // หน่วงก่อนปล่อย VFX (s)
    public float poisonStateDuration = 1.5f;
    private GameObject poisonInstance;
    [Header("Meteor Settings")]
    public GameObject meteorWarningPrefab;
    public GameObject meteorPrefab;
    public float meteorDamageRadius = 2f;
    public float meteorDamage = 25f;
    public float meteorDelay = 1.5f;        // หน่วงก่อนทำดาเมจ
    public float meteorCooldown = 8f;
    public float meteorRangeMin = 10f;
    public float meteorRangeMax = 25f;
    [Header("Health Bar UI")]
    [Tooltip("Panel ที่เก็บ Image ของ Health Bar")]
    public GameObject healthBarUI;
    [Tooltip("Image ที่ตั้งเป็น Filled (Horizontal)")]
    public Image     healthBarFill;
    
    [HideInInspector] public float lastMeteorTime = -Mathf.Infinity;

    public Vector3 Position => transform.position;
    public Vector3 PlayerPosition => Player.position;

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new NagaStateMachine();
        patrolState = new NagaPatrolState(this);
        chaseState = new NagaChaseState(this);
        attackState = new NagaAttackState(this);
        hurtState = new NagaHurtState(this);
        poisonState = new NagaPoisonState(this);
        meteorState = new NagaMeteorState(this);
        attackIdleState = new NagaAttackIdleState(this);

        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (healthBarUI != null)
            healthBarUI.SetActive(false);

        patrolCenter = transform.position;
    }

    void Start()
    {
        StateMachine.Initialize(patrolState);
    }

    void Update()
    {
        StateMachine.Update();
        TryCastMeteor();
        Animator.SetFloat("Speed", Agent.velocity.magnitude);
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        if (healthBarUI == null || healthBarFill == null)
            return;

        // เช็กระยะ chase
        bool inChase = IsPlayerInChaseRange();

        // โชว์/ซ่อน panel
        healthBarUI.SetActive(inChase);

        if (inChase)
        {
            // อัพเดต fillAmount (0–1)
            healthBarFill.fillAmount = HealthNormalized;
        }
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
    public void SpawnPoisonVFX()
    {
        if (poisonVFXPrefab == null || poisonSpawnPoint == null) return;
        poisonInstance = Instantiate(
            poisonVFXPrefab,
            poisonSpawnPoint.position,
            poisonSpawnPoint.rotation
        );
        NagaAudioManager.Instance.PlayOneShot(NagaAudioManager.Instance.poisonClip);
    }
    public bool CanUseMeteor()
    {
        float dist = Vector3.Distance(Position, PlayerPosition);
        return Time.time >= lastMeteorTime + meteorCooldown &&
            dist >= meteorRangeMin && dist <= meteorRangeMax;
    }

    private void TryCastMeteor()
    {
        if (IsDead || StateMachine.CurrentState == hurtState)
            return;

        if (CanUseMeteor() && StateMachine.CurrentState != meteorState)
        {
            lastMeteorTime = Time.time;
            StateMachine.ChangeState(meteorState);
        }
    }


    // เรียกจาก State เพื่อทำลาย VFX ตอนจบ
    public void DestroyPoisonVFX()
    {
        if (poisonInstance != null)
            Destroy(poisonInstance);
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
            float dmg = attackDamages[Mathf.Clamp(atkIndex - 1, 0, attackDamages.Length - 1)];
            stats.TakeDamage(dmg);
        }
    }
    protected override void OnDamageTaken(float damage)
    {
        base.OnDamageTaken(damage);
        // ถ้ายังไม่ตาย ให้เข้า Hurt state
        if (!IsDead)
            StateMachine.ChangeState(hurtState);

        NagaAudioManager.Instance.PlayOneShot(NagaAudioManager.Instance.hurtClip);
    }
    public override void OnDeath()
    {
        Agent.isStopped = true;
        Animator.SetTrigger("Die");
        NagaAudioManager.Instance.PlayOneShot(NagaAudioManager.Instance.dieClip);
        Destroy(gameObject, 2f); // ทำลายหลังจาก 2 วินาทีเพื่อให้อนิเมชั่นตายเล่นจบ
    }
}
