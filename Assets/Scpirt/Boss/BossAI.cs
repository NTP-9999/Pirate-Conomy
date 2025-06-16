using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum BossState
{
    Idle,
    Chasing,
    Attacking,
    Stunned,
    Dead
}

public class BossAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float stoppingDistance = 1.5f;
    public float rotationSpeed = 5f;
    public float agentMovementSpeed = 3.5f;

    [Header("Attack Cooldown Settings")]
    public float minAttackCooldown = 1f;
    public float maxAttackCooldown = 5f;
    public int numberOfAttackAnimations = 2;

    [Header("Stun Settings")]
    public float defaultStunDuration = 3.0f;

    [Header("Warp Settings")]
    public float warpCooldown = 5f; // Cooldown for general warp
    public float warpTriggerDistance = 12f; // If player is further than this, boss considers warping
    public GameObject warpVFXPrefab; // VFX to play when boss warps (at destination)
    // public AudioClip warpSFX; // <-- เราจะย้าย AudioClip ไปอยู่ใน WarpAudioSource โดยตรง

    public float navMeshSampleRadius = 2f; 

    [Header("Audio")]
    public AudioSource bossAudioSource; // Main AudioSource for general SFX (e.g., attack)
    public AudioSource warpAudioSource; // <-- NEW: Dedicated AudioSource for Warp SFX
    public AudioClip attackSFX;


    private Transform target;
    private NavMeshAgent agent;
    private BossStats bossStats;
    private Animator animator;
    private float lastAttackTime;
    private float currentAttackCooldown;

    private float lastWarpTime; 

    [SerializeField] private BossAttackCollider bossAttackCollider;

    public BossState CurrentState { get; private set; }
    private Coroutine stunRoutine;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target == null)
        {
            Debug.LogError("Player not found! Please ensure your Player GameObject has the tag 'Player'. Disabling BossAI.");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        bossStats = GetComponent<BossStats>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on BossAI! Disabling script.");
            enabled = false;
            return;
        }
        if (bossStats == null)
        {
            Debug.LogError("BossStats script not found on BossAI! Boss health/death won't function.");
        }
        if (animator == null)
        {
            Debug.LogError("Animator not found on BossAI! Boss animations won't play.");
        }

        agent.stoppingDistance = this.stoppingDistance;
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.speed = agentMovementSpeed;
        agent.acceleration = agentMovementSpeed * 2;

        if (bossAttackCollider == null)
        {
            bossAttackCollider = GetComponentInChildren<BossAttackCollider>();
            if (bossAttackCollider == null)
            {
                Debug.LogWarning("BossAttackCollider script not found on Boss or its children! Boss attacks won't deal damage.");
            }
        }

        // Try to find AudioSource for general SFX if not set in Inspector
        if (bossAudioSource == null)
        {
            bossAudioSource = GetComponent<AudioSource>();
            if (bossAudioSource == null)
            {
                Debug.LogWarning("AudioSource for general SFX not found on BossAI! Boss SFX might not play.");
            }
        }

        // NEW: Try to find AudioSource for warp SFX if not set in Inspector
        if (warpAudioSource == null)
        {
            // If main bossAudioSource is also null, try to add one for warp
            if (bossAudioSource == null)
            {
                warpAudioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("Added a new AudioSource for warp SFX as no general AudioSource was found.");
            }
            else
            {
                // If bossAudioSource exists, but warpAudioSource is not set,
                // consider whether you want to add another one or use the existing one.
                // For dedicated warp sound, it's better to add a new one.
                warpAudioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("Added a new AudioSource for warp SFX as it was not assigned.");
            }
        }

        // Configure new/existing warpAudioSource (ensure it's not set to PlayOnAwake or Loop)
        if (warpAudioSource != null)
        {
            warpAudioSource.playOnAwake = false;
            warpAudioSource.loop = false;
        }


        GenerateRandomAttackCooldown();
        lastAttackTime = -currentAttackCooldown; 
        lastWarpTime = Time.time - warpCooldown; 
        SetBossState(BossState.Idle); 
    }

    void Update()
    {
        if (bossStats != null && bossStats.IsDead)
        {
            SetBossState(BossState.Dead);
            return;
        }

        if (target == null)
        {
            SetBossState(BossState.Idle);
            return;
        }

        if (CurrentState == BossState.Stunned)
        {
            HandleStunnedState();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if ((CurrentState == BossState.Chasing || CurrentState == BossState.Attacking) &&
            distanceToTarget > warpTriggerDistance &&
            Time.time >= lastWarpTime + warpCooldown)
        {
            Debug.Log($"Boss in {CurrentState} state. Player too far ({distanceToTarget:F2} > {warpTriggerDistance:F2}) and warp cooldown ready. Attempting instant warp.");
            PerformInstantWarp();
            lastWarpTime = Time.time; 
        }

        switch (CurrentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Chasing:
                HandleChasingState();
                break;
            case BossState.Attacking:
                HandleAttackingState();
                break;
            case BossState.Dead:
                break;
        }
    }

    void SetBossState(BossState newState)
    {
        if (CurrentState == newState) return;

        Debug.Log($"Boss State Changed from {CurrentState} to {newState}");
        CurrentState = newState;

        switch (CurrentState)
        {
            case BossState.Idle:
            case BossState.Dead:
                StopAgentAndMovementAnimation(); 
                StopAllAttackEffectsFailsafe(); 
                break;
            case BossState.Attacking:
                StopAgentAndMovementAnimation(); 
                TryAttack(); 
                break;
            case BossState.Chasing:
                EnableAgentMovement(); 
                break;
            case BossState.Stunned:
                StopAgentAndMovementAnimation(); 
                StopAllAttackEffectsFailsafe();
                animator.SetTrigger("Stun"); 
                break;
        }
    }

    void HandleIdleState()
    {
        StopAgentAndMovementAnimation();
        RotateTowardsTarget(); 

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + currentAttackCooldown)
        {
            SetBossState(BossState.Attacking); 
        }
        else if (distanceToTarget <= chaseRange)
        {
            SetBossState(BossState.Chasing);
        }
    }

    void HandleChasingState()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        RotateTowardsTarget(); 

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + currentAttackCooldown)
        {
            SetBossState(BossState.Attacking); 
            return;
        }
        else if (distanceToTarget <= stoppingDistance + 0.1f) 
        {
            SetBossState(BossState.Idle); 
            return;
        }
        else if (distanceToTarget <= chaseRange)
        {
            EnableAgentMovement(); 
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position); 
                animator.SetBool("isMoving", true); 
            }
        }
        else 
        {
            SetBossState(BossState.Idle); 
        }
    }

    void HandleAttackingState()
    {
        StopAgentAndMovementAnimation(); 
        RotateTowardsTarget(); 
    }

    void HandleStunnedState()
    {
        // Boss does nothing while stunned.
    }

    void StopAgentAndMovementAnimation()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true; 
                agent.velocity = Vector3.zero; 
                agent.ResetPath(); 
            }
        }
        if (animator != null)
        {
            animator.SetBool("isMoving", false); 
        }
    }

    void EnableAgentMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            if (agent.isStopped)
            {
                agent.isStopped = false; 
                Debug.Log("NavMeshAgent enabled for movement.");
            }
        }
    }

    void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; 
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + currentAttackCooldown)
        {
            lastAttackTime = Time.time; 
            GenerateRandomAttackCooldown(); 
            Debug.Log($"Boss performs attack! Next cooldown: {currentAttackCooldown:F1}s");

            int randomIndex = Random.Range(0, numberOfAttackAnimations);
            animator.SetInteger("AttackIndex", randomIndex);
            animator.SetTrigger("Attack");
        }
    }

    private void GenerateRandomAttackCooldown()
    {
        currentAttackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        Debug.Log($"New attack cooldown generated: {currentAttackCooldown:F1} seconds");
    }

    public void ForceNewRandomCooldown()
    {
        GenerateRandomAttackCooldown();
    }

    public float GetCurrentAttackCooldown()
    {
        return currentAttackCooldown;
    }

    public float GetRemainingCooldownTime()
    {
        float remaining = (lastAttackTime + currentAttackCooldown) - Time.time;
        return Mathf.Max(0f, remaining); 
    }

    public void SetAttackingState(bool attacking)
    {
        if (CurrentState == BossState.Stunned || bossStats.IsDead) return;

        if (attacking)
        {
            SetBossState(BossState.Attacking);
        }
        else 
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange + 0.1f)
            {
                SetBossState(BossState.Idle); 
            }
            else
            {
                SetBossState(BossState.Chasing); 
            }
        }
    }

    public void PlayAttackSFX()
    {
        if (bossAudioSource != null && attackSFX != null)
        {
            bossAudioSource.PlayOneShot(attackSFX);
            Debug.Log("Anim Event: Attack SFX Played!");
        }
        else
        {
            if (bossAudioSource == null) Debug.LogWarning("PlayAttackSFX: bossAudioSource is null.");
            if (attackSFX == null) Debug.LogWarning("PlayAttackSFX: attackSFX (AudioClip) is null.");
        }
    }

    public void StopAllAttackEffectsFailsafe()
    {
        if (bossAudioSource != null && bossAudioSource.isPlaying)
        {
            bossAudioSource.Stop();
        }
        // NEW: Stop warp audio if it's playing
        if (warpAudioSource != null && warpAudioSource.isPlaying)
        {
            warpAudioSource.Stop();
        }

        if (bossAttackCollider != null)
        {
            bossAttackCollider.DisableHitbox(); 
        }
    }

    public void StunBoss(float duration)
    {
        if (bossStats.IsDead || CurrentState == BossState.Stunned) return;

        float actualStunDuration = (duration > 0) ? duration : defaultStunDuration;

        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(StunRoutine(actualStunDuration)); 
        SetBossState(BossState.Stunned); 
        Debug.Log($"Boss is entering Stun State for {actualStunDuration} seconds.");
    }

    IEnumerator StunRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (bossStats.IsDead) yield break;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            SetBossState(BossState.Idle); 
        }
        else if (distanceToTarget <= chaseRange)
        {
            SetBossState(BossState.Chasing); 
        }
        else
        {
            SetBossState(BossState.Idle); 
        }
        Debug.Log("Boss stun duration ended. Returning to normal state.");
        stunRoutine = null; 
    }

    void PerformInstantWarp()
    {
        Vector3 finalWarpPosition = Vector3.zero;
        bool positionFound = false;

        if (target == null)
        {
            Debug.LogWarning("Cannot perform instant warp: Player target is null.");
            return;
        }

        bool agentWasEnabled = false;
        if (agent != null && agent.enabled)
        {
            agentWasEnabled = true;
            agent.enabled = false;
            Debug.Log("NavMeshAgent temporarily disabled for instant warp.");
        }
        
        Vector3 desiredWarpPosition = target.position + target.forward * stoppingDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredWarpPosition, out hit, navMeshSampleRadius, NavMesh.AllAreas))
        {
            finalWarpPosition = hit.position;
            positionFound = true;
            Debug.Log($"Found valid NavMesh position in front of player: {finalWarpPosition}");
        }
        else
        {
            Debug.LogWarning($"Could not find valid NavMesh position near {desiredWarpPosition} (in front of player). Boss will not warp.");
        }

        if (positionFound)
        {
            transform.position = finalWarpPosition; 
            RotateTowardsTarget(); 

            if (warpVFXPrefab != null)
            {
                GameObject vfxInstance = Instantiate(warpVFXPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
                if (ps != null) Destroy(vfxInstance, ps.main.duration + 0.1f);
                else Destroy(vfxInstance, 2.0f); 
            }
            // NEW: Play Warp SFX using the dedicated warpAudioSource
            if (warpAudioSource != null && warpAudioSource.clip != null) // Check if warpAudioSource and its clip are assigned
            {
                warpAudioSource.PlayOneShot(warpAudioSource.clip); // Play the assigned clip
                Debug.Log("Warp SFX Played!");
            }
            else
            {
                if (warpAudioSource == null) Debug.LogWarning("PerformInstantWarp: warpAudioSource is null.");
                else if (warpAudioSource.clip == null) Debug.LogWarning("PerformInstantWarp: warpAudioSource.clip (Warp SFX) is null. Please assign an AudioClip to the Warp Audio Source component in Inspector.");
            }

            Debug.Log($"Boss instantly warped to {finalWarpPosition}");
        }
        else
        {
            // If warp failed, nothing happens. Log already handled.
        }

        if (agentWasEnabled)
        {
            if (agent != null && !agent.enabled) 
            {
                 agent.enabled = true;
            }
            agent.ResetPath();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && agent != null)
        {
            Gizmos.color = Color.cyan;
            if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            {
                Gizmos.DrawWireSphere(agent.destination, 0.2f);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, warpTriggerDistance);

        if (target != null)
        {
            Vector3 desiredWarpPosition = target.position + target.forward * stoppingDistance;
            Gizmos.color = new Color(0f, 1f, 0f, 0.7f);
            Gizmos.DrawWireSphere(desiredWarpPosition, 0.3f);
            Gizmos.DrawLine(target.position, desiredWarpPosition);

            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(desiredWarpPosition, navMeshSampleRadius);
        }
    }
}