using UnityEngine;

public class CollectFragmentState : IState
{
    PlayerStateMachine sm;
    public CollectFragmentState(PlayerStateMachine sm) { this.sm = sm; }

    public void Enter()
    {
        var frag = sm.currentFragment;
        if (frag != null) {
            // เก็บข้อมูลเข้าตัว player
            sm.AddFragment(frag.fragmentID, frag.amount);
            if (frag.interactUI != null)
            frag.interactUI.SetActive(false);
            // เลิกใช้งานชิ้นส่วน
            Object.Destroy(frag.gameObject);
        }

        // เคลียร์เป้าและกลับ Idle
        sm.currentFragment = null;
        sm.fsm.ChangeState(sm.idleState);
    }

    public void Execute() { }
    public void Exit()    { }
}
