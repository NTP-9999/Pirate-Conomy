using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState;
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;
    public PlayerJumpState jumpState;
    public PlayerController playerController;
    public PlayerHurtState hurtState;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        attackState = new PlayerAttackState(this);
        jumpState = new PlayerJumpState(this);
        hurtState = new PlayerHurtState(this);
    }

    void Start()
    {
        ChangeState(idleState);
    }

    void Update()
    {
        currentState?.LogicUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
}
