using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public EnemyStateMachine stateMachine { get; private set; }

    public IdleState idleState { get; private set; }
    public PatrolState patrolState { get; private set; }
    public ChaseState chaseState { get; private set; } // 🆕 เพิ่ม
    public AttackState attackState { get; private set; }

    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;

    void Awake()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        agent.speed = walkSpeed;
        
        stateMachine = new EnemyStateMachine();
        attackState = new AttackState(this, stateMachine);
        idleState = new IdleState(this, stateMachine);
        patrolState = new PatrolState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine); // 🆕
    }

    void Start()
    {
        agent.updateRotation = true;
        // หาผู้เล่น (ต้อง Tag player เป็น "Player")
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.currentState?.LogicUpdate();
    }
    public IEnumerator WaitEndAttack()
    {
        yield return new WaitForSeconds(0.8f);
        agent.isStopped = false;
    }
}
