using UnityEngine.AI;
using UnityEngine;

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
}
