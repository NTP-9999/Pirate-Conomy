using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class OilResource : MonoBehaviour
{
    public string displayName = "Oil";
    public int maxCollects = 3;
    public int currentCollects;
    public float respawnDelay = 5f;
    public ResourceInteractUI interactUI;
    public Transform interactPoint;
    private SphereCollider sphereCollider;
    private float interactShowRange => sphereCollider.radius;
    private float interactableRange => interactShowRange * .75f;

    MeshRenderer meshRenderer;
    bool playerInRange;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        if (interactPoint == null)
        {
            interactPoint = transform;
        }
    }

    public void Collect()
    {
        currentCollects++;
        if (currentCollects >= maxCollects)
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
        currentCollects = 0;
        meshRenderer.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI = InteractableUIManager.Instance.CreateResourceInteractUI(interactPoint).GetComponent<ResourceInteractUI>();
            interactUI.SetUp(displayName);
            playerInRange = true;
            other.GetComponent<PlayerStateMachine>().currentOil = this;
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
            if (psm.currentOil == this) psm.currentOil = null;
        }
    }
}
