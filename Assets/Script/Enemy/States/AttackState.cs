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
        Attack();           // ตีทันทีตอนเข้า state
        timer = 0f;         // รีเซ็ต timer หลังจากตี
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
            Attack();       // ตีอีกครั้ง
            timer = 0f;     // รีเซ็ต timer
        }
    }

    private void Attack()
    {
        enemy.agent.isStopped = true;
        enemy.animator.SetTrigger("Attack");
        if (CharacterStats.Instance != null)
        {
            CharacterStats.Instance.TakeDamage(Atkdamage);
            Debug.Log("Player took damage from enemy attack amount: " + Atkdamage);
        }
        enemy.StartCoroutine(enemy.WaitEndAttack());
    }
    
    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit Attack State");
        enemy.agent.isStopped = false;
    }
}
