using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class TreeTarget : MonoBehaviour
{
    [Header("Settings")]
    public int maxChops = 5;
    [HideInInspector] public int currentChops = 0;

    [Header("Respawn")]
    public float respawnDelay = 5f;

    [Header("UI")]
    public GameObject interactUI;     // “Press E”

    public Sprite woodIcon; // ให้ FSM ดึงมาใส่ Inventory

    private MeshRenderer meshRenderer;
    private Collider      boxCollider;
    private Collider      capsuleCollider;
    private bool          playerInRange;

    void Start()
    {
        meshRenderer  = GetComponent<MeshRenderer>();
        boxCollider   = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (interactUI != null) interactUI.SetActive(false);
    }

    public void Chop()
    {
        currentChops++;
        Debug.Log($"Tree hit: {currentChops}/{maxChops}");

        if (currentChops >= maxChops)
        {
            Debug.Log("Tree fell!");
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
            if (interactUI != null) interactUI.SetActive(false);
            StartCoroutine(RespawnTree());
        }
    }

    IEnumerator RespawnTree()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentChops = 0;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        capsuleCollider.enabled = true;
        if (playerInRange && interactUI != null)
            interactUI.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerInRange && other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null && meshRenderer.enabled)
                interactUI.SetActive(true);
            // บอก FSM เป้าหมายตัวนี้
            other.GetComponent<PlayerStateMachine>().currentTree = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (playerInRange && other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm.currentTree == this) psm.currentTree = null;
        }
    }
}
