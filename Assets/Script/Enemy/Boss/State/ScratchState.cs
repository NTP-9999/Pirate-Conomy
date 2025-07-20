using UnityEngine;

public class ScratchState : BossAttackState
{
    // duration kept at 1.8s as before
    public ScratchState(IceDragonBossAI boss) : base(boss, 1.8f) { }

    public override void Enter()
    {
        base.Enter();
        // fire your “Scratch” animation
        boss.animator.SetTrigger("Scratch");
        // TODO: apply melee scratch damage here if you want
    }
}