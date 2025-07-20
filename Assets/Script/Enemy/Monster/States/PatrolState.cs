using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState
{
    private float wanderRadius = 10f; // รัศมีที่เดินมั่วๆ
    private float wanderTimer = 0f;
    private float wanderInterval = 2f; // เดินมั่วทุกกี่วิ

    public PatrolState(EnemyController enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Patrol State");
        wanderTimer = wanderInterval;
        PickRandomDestination();
        enemy.animator.SetBool("Idle", false); // เผื่อมี animator
        enemy.animator.SetBool("isRunning", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval || enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            PickRandomDestination();
            wanderTimer = 0f;
        }

        // ถ้าเห็นผู้เล่น → เปลี่ยน state เป็น chase
        if (enemy.player != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);
            if (distance < 5f)
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
    }

    private void PickRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += enemy.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            enemy.agent.SetDestination(hit.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit Patrol State");
        enemy.animator.SetBool("isRunning", false);
    }
}
