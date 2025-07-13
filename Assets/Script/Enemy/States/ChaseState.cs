using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyController enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Chase State");
        enemy.animator.SetBool("Chasing", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.player != null)
        {
            enemy.agent.SetDestination(enemy.player.position);

            float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
            if (distance <= enemy.agent.stoppingDistance)
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit Chase State");
    }
}
