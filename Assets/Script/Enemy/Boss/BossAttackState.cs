using UnityEngine;

public abstract class BossAttackState : BossIState
{
    protected readonly IceDragonBossAI boss;
    protected float timer;
    protected readonly float duration;

    // Constructor name matches class name:
    public BossAttackState(IceDragonBossAI boss, float duration)
    {
        this.boss     = boss;
        this.duration = duration;
    }

    public virtual void Enter()
    {
        boss.ResetAttackCooldown();
        boss.FacePlayer();
        timer = 0f;
    }

    public virtual void Tick()
    {
        timer += Time.deltaTime;
        boss.FacePlayer();

        if (timer >= duration)
            boss.stateMachine.ChangeState(new BossIdleState(boss));
    }

    public virtual void Exit() { }
}
