using UnityEngine;

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

    private void Start()
    {
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
            if (interactUI != null)
                interactUI.SetActive(false);

            // สั่งเกิดใหม่
            TreeRespawner.Instance.RespawnTree(oilPrefab, transform.position, transform.rotation, respawnDelay);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null)
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
