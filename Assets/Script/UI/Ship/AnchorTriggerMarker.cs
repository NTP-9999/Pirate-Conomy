using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public string displayName = "Anchor";
    public string actionName = "Cast Anchor";
    public Sprite keySprite => shipAnchorSystem.gImage.sprite;
    public ShipAnchorSystem shipAnchorSystem;
    [HideInInspector]public OtherInteractUI interactUI;
    public Transform interactPoint;
    private float maxDistance = 0;
    private float interactableRange => maxDistance * 0.75f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (maxDistance == 0)
            {
                maxDistance = //Distance from ship to player
                    Vector3.Distance(this.transform.position, other.transform.position);
            }
            interactUI = InteractableUIManager.Instance.CreateOtherInteractUI(interactPoint).GetComponent<OtherInteractUI>();
            interactUI.SetUp(displayName, actionName, keySprite,KeyCode.G, shipAnchorSystem.holdTimeToTrigger);
            shipAnchorSystem.SetAnchorZoneState(true);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && interactUI != null)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            if (distance > interactableRange && interactUI.interactUIState != InteractUIState.ShowInteractable)
            {
                interactUI.ReturnToShowInteractable();
            }
            else if (distance <= interactableRange && interactUI.interactUIState != InteractUIState.Interactable)
            {
                interactUI.Interactable();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.HideUI();
            shipAnchorSystem.SetAnchorZoneState(false);
        }
    }
}