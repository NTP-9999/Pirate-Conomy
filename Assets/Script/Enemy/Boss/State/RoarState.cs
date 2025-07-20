using UnityEngine;

// … put this in the same folder as your other states …
public class RoarState : BossAttackState
{
    // keep the same duration you had for IceBreath
    public RoarState(IceDragonBossAI boss) : base(boss, 2.0f) { }

    public override void Enter()
    {
        base.Enter();
        // trigger your “Roar” animation
        boss.animator.SetTrigger("Roar");
        // no damage logic here—just play the animation
    }
}
