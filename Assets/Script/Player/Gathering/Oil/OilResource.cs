using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class OilResource : MonoBehaviour
{
    public int maxCollects = 3;
    public int currentCollects;
    public float respawnDelay = 5f;
    public GameObject interactUI;

    MeshRenderer meshRenderer;
    bool playerInRange;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (interactUI != null) interactUI.SetActive(false);
    }

    public void Collect()
    {
        currentCollects++;
        if (currentCollects >= maxCollects)
        {
            meshRenderer.enabled = false;
            StartCoroutine(Respawn());
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentCollects = 0;
        meshRenderer.enabled = true;
        if (playerInRange && interactUI != null) interactUI.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI!=null && meshRenderer.enabled)
                interactUI.SetActive(true);
            other.GetComponent<PlayerStateMachine>().currentOil = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI!=null) interactUI.SetActive(false);
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm.currentOil == this) psm.currentOil = null;
        }
    }
}
