using UnityEngine;

public class TailSwipeState : BossAttackState
{
    public TailSwipeState(IceDragonBossAI boss) : base(boss, 1.5f) { }

    public override void Enter()
    {
        base.Enter();
        boss.animator.SetTrigger("TailSwipe");
        // TODO: detect melee hit in an arc in front of the dragon
    }
}
