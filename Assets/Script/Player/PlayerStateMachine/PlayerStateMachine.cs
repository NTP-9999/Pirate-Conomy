using UnityEngine;
using System.Collections.Generic;

public class PlayerStateMachine : MonoBehaviour
{
    public StateMachine fsm;
    public PlayerController playerController;
    public int GetFragmentCount(string id)
        => fragmentCounts.TryGetValue(id, out var c) ? c : 0;

    [HideInInspector] public PlayerState currentState;

    // Fragment
    [HideInInspector] public FragmentResource currentFragment;
    [HideInInspector] public CollectFragmentState collectFragmentState;

    // Tree
    [HideInInspector] public TreeChopper  treeChopper;
    [HideInInspector] public TreeTarget   currentTree;
    [HideInInspector] public CollectTreeState collectTreeState;

    // Oil
    [HideInInspector] public OilCollector oilCollector;
    [HideInInspector] public OilResource  currentOil;
    [HideInInspector] public CollectOilState collectOilState;

    // Ore
    [HideInInspector] public OreCollector oreCollector;
    [HideInInspector] public OreResource  currentOre;
    [HideInInspector] public CollectOreState collectOreState;

    // --- Attack ---
    public float maxComboDelay = 0.5f;
    [HideInInspector] public bool comboInputBuffered;
    [HideInInspector] public PlayerAttack1State    attack1State;
    [HideInInspector] public PlayerAttack2State    attack2State;
    [HideInInspector] public PlayerAttack3State    attack3State; 

    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerHurtState hurtState;
    [HideInInspector] public PlayerSwimState swimState;
    [HideInInspector] public PlayerRollState rollState;

    void Awake()
    {
        fsm                = new StateMachine();
        collectFragmentState = new CollectFragmentState(this);

        playerController = GetComponent<PlayerController>();
        oilCollector = GetComponent<OilCollector>();
        oreCollector = GetComponent<OreCollector>();
        treeChopper = GetComponent<TreeChopper>();
        
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        attack1State = new PlayerAttack1State(this);
        attack2State = new PlayerAttack2State(this);
        attack3State = new PlayerAttack3State(this);
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

    // Fragment count & unlock
    Dictionary<string,int> fragmentCounts = new Dictionary<string,int>();
    public void AddFragment(string id, int amt) {
        if (!fragmentCounts.ContainsKey(id))
            fragmentCounts[id] = 0;
        fragmentCounts[id] += amt;
        Debug.Log($"Fragment {id}: now {fragmentCounts[id]}");
        SkillManager.Instance.TryUnlockSkill(id, fragmentCounts[id]);
    }
}
