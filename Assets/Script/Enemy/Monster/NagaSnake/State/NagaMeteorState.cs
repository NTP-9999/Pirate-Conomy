using UnityEngine;

public class NagaMeteorState : NagaIState
{
    private SnakeNagaAI ctx;
    private Vector3 targetPosition;
    private float enterTime;
    private bool damageApplied = false;
    private GameObject warningInstance;

    public NagaMeteorState(SnakeNagaAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        ctx.Agent.isStopped = true;
        ctx.Animator.SetTrigger("Attack4");

        // ล็อกท่าทางและตำแหน่ง
        targetPosition = ctx.PlayerPosition;
        enterTime = Time.time;
        damageApplied = false;

        // วงแดงเตือน
        if (ctx.meteorWarningPrefab != null)
        {
            warningInstance = GameObject.Instantiate(
                ctx.meteorWarningPrefab,
                targetPosition,
                Quaternion.identity
            );
        }

        // Meteor VFX โผล่ทันที
        if (ctx.meteorPrefab != null)
        {
            GameObject.Instantiate(
                ctx.meteorPrefab,
                targetPosition,
                Quaternion.identity
            );
            CameraShake.Instance.Shake(6f, 0.7f);
        }
    }

    public void Execute()
    {
        float elapsed = Time.time - enterTime;

        // รอจนถึงเวลาทำดาเมจ
        if (!damageApplied && elapsed >= ctx.meteorDelay)
        {
            damageApplied = true;

            if (warningInstance != null)
                GameObject.Destroy(warningInstance);

            float distance = Vector3.Distance(ctx.PlayerPosition, targetPosition);
            if (distance <= ctx.meteorDamageRadius)
            {
                var stats = ctx.Player.GetComponent<CharacterStats>();
                if (stats != null)
                    stats.TakeDamage(ctx.meteorDamage);
            }

            // กลับ state ตามสถานการณ์
            if (ctx.IsPlayerInAttackRange())
                ctx.StateMachine.ChangeState(ctx.attackState);
            else if (ctx.IsPlayerInChaseRange())
                ctx.StateMachine.ChangeState(ctx.chaseState);
            else
                ctx.StateMachine.ChangeState(ctx.patrolState);
        }
    }

    public void Exit()
    {
        if (warningInstance != null)
            GameObject.Destroy(warningInstance);
    }
}
