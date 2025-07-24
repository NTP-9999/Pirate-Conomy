using UnityEngine;
using System.Collections;

public class CollectOreState : IState
{
    PlayerStateMachine sm;
    public CollectOreState(PlayerStateMachine sm){this.sm=sm;}

    public void Enter()
        => sm.StartCoroutine(Routine());
    IEnumerator Routine()
    {
        OreResource ore = sm.currentOre; // 🔒 ล็อกไว้กันโดนเปลี่ยนจากภายนอก

        if (ore == null || sm.oreCollector == null)
        {
            Debug.LogError("Ore or OreCollector is null.");
            yield break;
        }

        yield return sm.oreCollector.StartMineFromExternal(ore);

        if (ore.currentHits >= ore.maxHits)
            sm.currentOre = null;

        sm.fsm.ChangeState(sm.idleState);
    }
    public void Execute(){} public void Exit(){}
}
