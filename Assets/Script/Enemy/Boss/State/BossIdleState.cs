using UnityEngine;

public class BossIdleState : BossIState
{
    private readonly IceDragonBossAI boss;

    // Constructor name matches class name:
    public BossIdleState(IceDragonBossAI boss)
    {
        this.boss = boss;
    }

    public void Enter()
    {
        boss.animator.ResetTrigger("IceBreath");
        boss.animator.ResetTrigger("TailSwipe");
        boss.animator.ResetTrigger("WingSlam");
        boss.animator.Play("Idle");
    }

    public void Tick()
    {
        if (!boss.PlayerInRange()) return;
        boss.FacePlayer();

        if (!boss.CanAttack()) return;
        int choice = Random.Range(0, 3);
        switch (choice)
        {
            case 0:
                boss.stateMachine.ChangeState(new IceBreathState(boss));
                break;
            case 1:
                boss.stateMachine.ChangeState(new TailSwipeState(boss));
                break;
            case 2:
                boss.stateMachine.ChangeState(new WingSlamState(boss));
                break;
        }
    }

    public void Exit() { }
}
