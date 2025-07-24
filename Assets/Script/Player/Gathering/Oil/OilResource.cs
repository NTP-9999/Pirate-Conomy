using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OilResource : MonoBehaviour
{
    public string displayName = "Oil";
    public int maxCollects = 3;
    public int currentCollects;
    public float respawnDelay = 5f;
    [HideInInspector]public ResourceInteractUI interactUI;
    public Transform interactPoint;
    private SphereCollider sphereCollider;
    private float interactShowRange => sphereCollider.radius;
    private float interactableRange => maxDistance * .75f;
    private float maxDistance = 0;

    bool playerInRange;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
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
            StartCoroutine(Respawn());
            interactUI.HideUI();
            interactUI = null;
            gameObject.SetActive(false); // Disable the oil object instead of destroying it
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentCollects = 0;
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
