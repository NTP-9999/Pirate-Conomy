using UnityEngine;
using System.Collections;

public class CollectOreState : IState
{
    PlayerStateMachine sm;

    public CollectOreState(PlayerStateMachine sm)
    {
        this.sm = sm;
    }

    public void Enter()
    {
        // เรียก playerController ไม่ใช่ controller
        sm.playerController.canMove = false;
        sm.playerController.animator.SetTrigger("Mine");

        // เริ่ม Coroutine เก็บน้ำมัน
        sm.StartCoroutine(Collect());
    }

    IEnumerator Collect()
    {
        yield return new WaitForSeconds(1.5f);

        // PerformCollect ใช้ sm.oilCollector และ sm.currentOil
        sm.oreCollector.PerformMine(sm.currentOre);

        // คืนสิทธิ์การเคลื่อนที่
        sm.playerController.canMove = true;

        // กลับไป IdleState
        sm.fsm.ChangeState(sm.idleState);
    }

    public void Execute() { }
    public void Exit()    { }
}
