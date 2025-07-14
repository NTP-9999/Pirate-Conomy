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
        yield return sm.oreCollector.StartMineFromExternal(sm.currentOre);
        if (sm.currentOre.currentHits >= sm.currentOre.maxHits)
            sm.currentOre = null;
        sm.fsm.ChangeState(sm.idleState);
    }
    public void Execute(){} public void Exit(){}
}
