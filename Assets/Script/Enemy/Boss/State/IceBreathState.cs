using UnityEngine;

public class IceBreathState : BossAttackState
{
    public IceBreathState(IceDragonBossAI boss) : base(boss, 2.0f) { }

    public override void Enter()
    {
        base.Enter();
        boss.animator.SetTrigger("IceBreath");
        // TODO: spawn ice‐breath VFX/projectile or call BossStat.TakeDamage()
    }
}
