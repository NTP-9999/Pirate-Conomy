using UnityEngine;

public class SwimZoneTrigger : MonoBehaviour
{
    private bool isPlayerInWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayerInWater && other.CompareTag("Player"))
        {
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                psm.fsm.ChangeState(psm.swimState);
                isPlayerInWater = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerInWater && other.CompareTag("Player"))
        {
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                // กลับ IdleState
                psm.fsm.ChangeState(psm.idleState);
                isPlayerInWater = false;
            }
        }
    }
}
