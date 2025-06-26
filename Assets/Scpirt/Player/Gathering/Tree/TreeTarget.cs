using UnityEngine;
using UnityEngine.UI;

public class TreeTarget : MonoBehaviour
{
    [Header("Tree Settings")]
    public int maxChops = 5;
    private int currentChops = 0;

    [Header("UI")]
    public GameObject interactUI; // UI "Press E"
    
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
            interactUI.SetActive(false);
            Debug.Log("Tree fell!");
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
