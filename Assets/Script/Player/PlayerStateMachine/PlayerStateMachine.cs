using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector] public StateMachine fsm;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerState currentState;

    [HideInInspector] public OilCollector  oilCollector;
    [HideInInspector] public OilResource   currentOil;
    [HideInInspector] public CollectOilState collectOilState;

    [HideInInspector] public OreCollector  oreCollector;
    [HideInInspector] public OreResource   currentOre;
    [HideInInspector] public CollectOreState collectOreState;

    [HideInInspector] public TreeChopper  treeChopper;
    [HideInInspector] public TreeTarget  currentTree;
    [HideInInspector] public CollectTreeState collectTreeState;


    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerAttackState attackState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerHurtState hurtState;
    [HideInInspector] public PlayerSwimState swimState;
    [HideInInspector] public PlayerRollState rollState;

    void Awake()
    {
        fsm = new StateMachine();


        playerController = GetComponent<PlayerController>();
        oilCollector     = GetComponent<OilCollector>();
        oreCollector     = GetComponent<OreCollector>();
        treeChopper     = GetComponent<TreeChopper>();
        
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);
        jumpState = new PlayerJumpState(this);
        hurtState = new PlayerHurtState(this);
        swimState = new PlayerSwimState(this);
        rollState = new PlayerRollState(this);
        
        collectOilState = new CollectOilState(this);
        collectOreState = new CollectOreState(this);
        collectTreeState = new CollectTreeState(this);
    }
    void Start() => fsm.Initialize(idleState);
    void Update() => fsm.Update();
}
