using UnityEngine;

public class WingSlamState : BossAttackState
{
    public WingSlamState(IceDragonBossAI boss) : base(boss, 1.8f) { }

    public override void Enter()
    {
        base.Enter();
        boss.animator.SetTrigger("WingSlam");
        // TODO: apply AoE damage around the dragon
    }
}
