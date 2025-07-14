using UnityEngine;
using System.Collections;

public class CollectTreeState : IState
{
    private PlayerStateMachine sm;

    public CollectTreeState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        // สั่ง Coroutine ให้ TreeChopper ฟันต้นไม้
        sm.StartCoroutine(ChopRoutine());
    }

    private IEnumerator ChopRoutine()
    {
        // รอจน TreeChopper.StartChopFromExternal จบ (ล็อก input, แอนิเม เสร็จ, PerformChop)
        yield return sm.treeChopper.StartChopFromExternal(sm.currentTree);

        if (sm.currentTree.currentChops >= sm.currentTree.maxChops)
            sm.currentTree = null;
        sm.fsm.ChangeState(sm.idleState);
    }

    public void Execute() { }
    public void Exit()    { }
}
