using UnityEngine;
using System.Collections;

public class CollectOilState : IState
{
    PlayerStateMachine sm;
    public CollectOilState(PlayerStateMachine sm){this.sm=sm;}

    public void Enter()
        => sm.StartCoroutine(Routine());
    IEnumerator Routine()
    {
        yield return sm.oilCollector.StartCollectFromExternal(sm.currentOil);
        // ถ้าหมดแล้ว clear เป้า
        if (sm.currentOil.currentCollects >= sm.currentOil.maxCollects)
            sm.currentOil = null;
        sm.fsm.ChangeState(sm.idleState);
    }
    public void Execute(){} public void Exit(){}
}
