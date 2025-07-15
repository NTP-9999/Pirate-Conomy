using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
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
    private float interactableRange => interactShowRange * .75f;

    MeshRenderer meshRenderer;
    bool playerInRange;

    void Start()
    {
        meshRenderer  = GetComponent<MeshRenderer>();
        oreCollider   = GetComponent<SphereCollider>();
    }

    public void Hit()
    {
        currentHits++;
        if (currentHits >= maxHits)
        {
            meshRenderer.enabled = false;
            StartCoroutine(Respawn());
            interactUI.HideUI();
            interactUI = null;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentHits = 0;
        meshRenderer.enabled  = true;
    }

    void OnTriggerEnter(Collider other)
    {
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
