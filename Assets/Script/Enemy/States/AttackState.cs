using UnityEngine;
using System.Collections;

public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float timer;
    private float Atkdamage = 10f;

    public AttackState(EnemyController enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Attack State");
        // ล็อกปลายทางของ NavMeshAgent ไว้ที่ตำแหน่งเดิม
        enemy.agent.SetDestination(enemy.transform.position);
        Attack();           
        timer = 0f;         
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timer += Time.deltaTime;
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance > enemy.agent.stoppingDistance + 0.5f)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
        else if (timer >= attackCooldown)
        {
            // ก่อนตีใหม่ ก็ล็อกตำแหน่งไว้ก่อนเหมือนเดิม
            enemy.agent.SetDestination(enemy.transform.position);
            Attack();
            timer = 0f;
        }
    }

    private void Attack()
    {
        // สั่งให้เล่นอนิเม Trigger:
        enemy.animator.SetTrigger("Attack");
        // ไม่ต้อง Call TakeDamage ตรงนี้อีก
        // เดี๋ยว Hitbox จะจัดการให้ตอน Activate() ทันที
        enemy.StartCoroutine(enemy.WaitEndAttack());
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit Attack State");
        // พอออก AttackState ก็เลิกล็อกปลายทาง ให้มันวิ่งตามปกติได้
        // (agent.isStopped ไม่เคยถูกเซ็ต true เลย จึงไม่ต้องเปลี่ยนอะไร)
    }
}

