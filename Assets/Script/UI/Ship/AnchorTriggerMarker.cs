using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public ShipAnchorSystem shipAnchorSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shipAnchorSystem.SetAnchorZoneState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shipAnchorSystem.SetAnchorZoneState(false);
        }
    }
}