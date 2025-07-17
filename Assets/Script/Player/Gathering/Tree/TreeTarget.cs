using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class TreeTarget : MonoBehaviour
{
    [Header("Settings")]
    public string displayName = "Tree";
    public int maxChops = 5;
    [HideInInspector] public int currentChops = 0;

    [Header("Respawn")]
    public float respawnDelay = 5f;

    [Header("UI")]
    public ResourceInteractUI interactUI;
    public Transform interactPoint;
    private CapsuleCollider TreeCollider;
    private SphereCollider sphereCollider;
    private float interactShowRange => sphereCollider.radius;
    private float interactableRange => maxDistance * .75f;
    private float maxDistance = 0;

    MeshRenderer meshRenderer;
    bool playerInRange;

    void Awake()
    {
        TreeCollider = GetComponent<CapsuleCollider>();
        meshRenderer  = GetComponentInChildren<MeshRenderer>();
        sphereCollider   = GetComponent<SphereCollider>();
    }
    void Start()
    {
        if (interactPoint == null)
        {
            interactPoint = transform;
        }
        // Try to find a child named "Point" and assign its transform, otherwise use this object's transform
        interactPoint = transform.Find("Point") != null ? transform.Find("Point") : transform;
    }

    public void Chop()
    {
        currentChops++;
        Debug.Log($"Tree hit: {currentChops}/{maxChops}");

        if (currentChops >= maxChops)
        {
            TreeCollider.enabled = false;
            meshRenderer.enabled = false;
            StartCoroutine(RespawnTree());
            interactUI.HideUI();
            interactUI = null;
        }
    }

    IEnumerator RespawnTree()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentChops = 0;
        TreeCollider.enabled = true;
        meshRenderer.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerInRange && other.CompareTag("Player"))
        {
            if (maxDistance == 0)
            {
                maxDistance = //Distance from ship to player
                    Vector3.Distance(interactPoint.transform.position, other.transform.position);
            }
            interactUI = InteractableUIManager.Instance.CreateResourceInteractUI(interactPoint).GetComponent<ResourceInteractUI>();
            interactUI.SetUp(displayName);
            playerInRange = true;
            // บอก FSM เป้าหมายตัวนี้
            other.GetComponent<PlayerStateMachine>().currentTree = this;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInRange && interactUI != null)
        {
            float distance = Vector3.Distance(other.transform.position, interactPoint.transform.position);
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
        if (playerInRange && other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.HideUI();
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm.currentTree == this) psm.currentTree = null;
        }
    }
}
