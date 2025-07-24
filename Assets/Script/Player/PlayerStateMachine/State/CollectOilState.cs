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
        if (sm == null || sm.oilCollector == null || sm.currentOil == null)
        {
            Debug.LogError("Initial null check failed");
            yield break;
        }

        // 🔒 เก็บ currentOil ไว้ในตัวแปร local
        OilResource oil = sm.currentOil;

        yield return sm.oilCollector.StartCollectFromExternal(oil);

        if (oil == null)
        {
            Debug.LogWarning("oil is null after collection");
            yield break;
        }

        if (oil.currentCollects >= oil.maxCollects)
            sm.currentOil = null;

        sm.fsm.ChangeState(sm.idleState);
    }

    public void Execute(){} public void Exit(){}
}
