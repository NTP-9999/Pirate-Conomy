using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OreResource : MonoBehaviour
{
    public string displayName = "Ore";
    public int maxHits = 5;
    public int currentHits;
    public float respawnDelay = 5f;
    [HideInInspector]public ResourceInteractUI interactUI;

    public Transform interactPoint;
    private SphereCollider oreCollider;
    private float interactShowRange => oreCollider.radius;
    private float interactableRange => maxDistance * .75f;
    private float maxDistance = 0;
    bool playerInRange;

    void Start()
    {
        oreCollider   = GetComponent<SphereCollider>();
    }

    public void Hit()
    {
        currentHits++;
        if (currentHits >= maxHits)
        {
            oreCollider.enabled = false;
            StartCoroutine(Respawn());

            if (interactUI != null)
                interactUI.HideUI();  // ✅ ป้องกัน null ที่อาจพังได้

            interactUI = null;
            gameObject.SetActive(false);
        }
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentHits = 0;
        oreCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (maxDistance == 0)
            {
                maxDistance = //Distance from ship to player
                    Vector3.Distance(interactPoint.transform.position, other.transform.position);
            }
        if (other.CompareTag("Player"))
        {
            interactUI = InteractableUIManager.Instance.CreateResourceInteractUI(interactPoint).GetComponent<ResourceInteractUI>();
            interactUI.SetUp(displayName);
            playerInRange = true;
            other.GetComponent<PlayerStateMachine>().currentOre = this;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInRange && interactUI != null)
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
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.HideUI();
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm.currentOre == this) psm.currentOre = null;
        }
    }
}
