using UnityEngine; 


public class IdleState : EnemyState
{
    private float idleTime = 0.8f;
    private float timer;

    public IdleState(EnemyController enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.ChangeState(enemy.patrolState);
        enemy.animator.SetBool("Patrol", true);
        timer = 0f;
        Debug.Log("Enter Idle State");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            enemy.animator.SetBool("Idle", false);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit Idle State");
    }
}
