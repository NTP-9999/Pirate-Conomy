using UnityEngine;

public class KravalonHurtState : IKravalonState
{
    private KravalonAI ctx;
    private float hurtDuration = 0.5f;
    private float enterTime;

    public KravalonHurtState(KravalonAI context)
    {
        ctx = context;
    }

    public void Enter()
    {
        enterTime = Time.time;
        // TODO: เล่น animation โดนยิง
        Debug.Log("Kravalon โดนยิง เจ็บ!");
    }

    public void Execute()
    {
        if (Time.time >= enterTime + hurtDuration)
        {
            // ถ้ายังอยู่ในระยะโจมตี → กลับไป Idle
            if (ctx.IsShipInAttackRange())
                ctx.StateMachine.ChangeState(ctx.attackIdleState);
            else
                ctx.StateMachine.ChangeState(ctx.chaseState);
        }
    }

    public void Exit() { }
}
