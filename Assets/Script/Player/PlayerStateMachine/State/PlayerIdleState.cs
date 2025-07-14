using UnityEngine;
public class PlayerIdleState : IState
{
    PlayerStateMachine sm;
    public PlayerIdleState(PlayerStateMachine sm) { this.sm = sm; }
    public void Enter()
    {
        sm.playerController.canMove = true;
        sm.playerController.animator.SetBool("IsMoving", false);
    }
    public void Execute()
    {
        // 1) Movement
        float h = Input.GetAxisRaw("Horizontal"),
              v = Input.GetAxisRaw("Vertical");
        bool moving = (Mathf.Abs(h)>0||Mathf.Abs(v)>0);
        if (Input.GetKeyDown(KeyCode.Space) && sm.playerController.IsGrounded()) { sm.fsm.ChangeState(sm.jumpState); return; }
        if (moving) { sm.fsm.ChangeState(sm.moveState); return; }

        // 2) Attack
        if (Input.GetMouseButtonDown(0)) { sm.fsm.ChangeState(sm.attackState); return; }

        // 4) Interact (Oil / Ore / Tree)
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            var origin = sm.transform.position + Vector3.up;
            if (Physics.Raycast(origin, sm.transform.forward, out hit, sm.playerController.attackRange))
            {
                if (hit.collider.TryGetComponent<OilResource>(out var oil))
                {
                    sm.currentOil = oil;
                    sm.fsm.ChangeState(sm.collectOilState);
                    return;
                }
                if (hit.collider.TryGetComponent<OreResource>(out var ore))
                {
                    sm.currentOre = ore;
                    sm.fsm.ChangeState(sm.collectOreState);
                    return;
                }
                if (hit.collider.TryGetComponent<TreeTarget>(out var tree))
                {
                    sm.currentTree = tree;
                    sm.fsm.ChangeState(sm.collectTreeState);
                    return;
                }
            }
        }
    }
    public void Exit() { }
}
