using UnityEngine;
using System.Collections;

public class TreeTarget : MonoBehaviour
{
    [Header("Tree Settings")]
    public int maxChops = 5;
    private int currentChops = 0;

    [Header("Respawn Settings")]
    public float respawnDelay = 5f; // เวลารอเกิดใหม่ (วินาที)
    public GameObject treePrefab;   // Prefab ของต้นไม้ (ลาก Prefab ของตัวเองมาใส่)

    [Header("UI")]
    public GameObject interactUI; // UI "Press E"

    private bool playerInRange = false;
    private MeshRenderer meshRenderer;
    private Collider treeCollider;

    private void Start()
    {
        treeCollider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TreeChopper chopper = GameObject.FindWithTag("Player").GetComponent<TreeChopper>();
            if (chopper != null)
            {
                StartCoroutine(chopper.StartChopFromExternal(this));
            }
        }
    }

    public void Chop()
    {
        currentChops++;
        Debug.Log($"Tree hit: {currentChops}/{maxChops}");

        if (currentChops >= maxChops)
        {
            Debug.Log("Tree fell!");

            if (interactUI != null)
                interactUI.SetActive(false);

            StartCoroutine(RespawnTree());
            meshRenderer.enabled = false;
            treeCollider.enabled = false; // Disable collider to prevent further interaction
            
        }
    }

    private IEnumerator RespawnTree()
    {
        yield return new WaitForSeconds(respawnDelay);
        meshRenderer.enabled = true;
        treeCollider.enabled = true; // Re-enable collider after respawn
        currentChops = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null && (meshRenderer == null || meshRenderer.enabled))
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}
