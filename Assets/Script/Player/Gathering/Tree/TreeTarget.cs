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
    private SphereCollider sphereCollider;
    private float interactShowRange => sphereCollider.radius;
    private float interactableRange => interactShowRange * .75f;

    MeshRenderer meshRenderer;
    bool playerInRange;

    void Awake()
    {
        meshRenderer  = GetComponent<MeshRenderer>();
        sphereCollider   = GetComponent<SphereCollider>();
    }
    void Start()
    {
        if (interactPoint == null)
        {
            interactPoint = transform;
        }
    }

    public void Chop()
    {
        currentChops++;
        Debug.Log($"Tree hit: {currentChops}/{maxChops}");

        if (currentChops >= maxChops)
        {
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
        meshRenderer.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerInRange && other.CompareTag("Player"))
        {
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
