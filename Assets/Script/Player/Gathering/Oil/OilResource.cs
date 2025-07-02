using UnityEngine;
using System.Collections;
public class OilResource : MonoBehaviour
{
    [Header("Settings")]
    public int maxCollects = 3;  // จำนวนครั้งที่เก็บได้
    private int currentCollects = 0;

    [Header("Respawn")]
    public float respawnDelay = 5f;
    public GameObject oilPrefab;

    [Header("UI")]
    public GameObject interactUI;

    private bool playerInRange = false;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OilCollector collector = GameObject.FindWithTag("Player").GetComponent<OilCollector>();
            if (collector != null)
            {
                StartCoroutine(collector.StartCollectFromExternal(this));
            }
        }
    }

    public void Collect()
    {
        currentCollects++;
        Debug.Log($"Oil collected: {currentCollects}/{maxCollects}");

        if (currentCollects >= maxCollects)
        {
            Debug.Log("Oil resource depleted!");
            if (interactUI != null)
                interactUI.SetActive(false);

            StartCoroutine(RespawnOil());
            meshRenderer.enabled = false;
            
        }
    }
    private IEnumerator RespawnOil()
    {
        Debug.Log("Respawning oil resource...");
        yield return new WaitForSeconds(respawnDelay);
        meshRenderer.enabled = true;
        currentCollects = 0;
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
