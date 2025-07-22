// AttackState.cs
using UnityEngine;

public class NagaAttackState : NagaIState {
    SnakeNagaAI ctx;
    float lastAtkTime;

    public NagaAttackState(SnakeNagaAI context) {
        ctx = context;
    }

    public void Enter() {
        ctx.Agent.isStopped = true;
    }

    public void Execute() {
        // หันหน้าเข้าหาผู้เล่น
        ctx.RotateTowardsPlayer();

        // เช็ค cooldown
        if (Time.time >= lastAtkTime + ctx.timeBetweenAttacks) {
            // สุ่ม 1–4 (1,2,3 = ท่าโจมตี; 4 = สกิลงูปล่อยพิษ)
            int choice = Random.Range(1, 5);

            if (choice <= 3) {
                // โยน trigger Attack1, Attack2, Attack3
                ctx.Animator.SetTrigger("Attack" + choice);
                lastAtkTime = Time.time;
            }
            else {
                // เปลี่ยนไปสกิลพิษแทน
                ctx.StateMachine.ChangeState(ctx.poisonState);
            }
        }

        // ถ้าผู้เล่นหลุดระยะ ให้กลับไปไล่ก่อน
        if (!ctx.IsPlayerInAttackRange())
            ctx.StateMachine.ChangeState(ctx.chaseState);
    }

    public void Exit() { }
}
