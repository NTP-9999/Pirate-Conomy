using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
public class OreResource : MonoBehaviour
{
    public int maxHits = 5;
    public int currentHits;
    public float respawnDelay = 5f;
    public GameObject interactUI;

    MeshRenderer meshRenderer;
    Collider oreCollider;
    bool playerInRange;

    void Start()
    {
        meshRenderer  = GetComponent<MeshRenderer>();
        oreCollider   = GetComponent<Collider>();
        if (interactUI!=null) interactUI.SetActive(false);
    }

    public void Hit()
    {
        currentHits++;
        if (currentHits >= maxHits)
        {
            meshRenderer.enabled  = false;
            oreCollider.enabled   = false;
            if (interactUI!=null) interactUI.SetActive(false);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        currentHits = 0;
        meshRenderer.enabled  = true;
        oreCollider.enabled   = true;
        if (playerInRange && interactUI!=null) interactUI.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI!=null && meshRenderer.enabled)
                interactUI.SetActive(true);
            other.GetComponent<PlayerStateMachine>().currentOre = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI!=null) interactUI.SetActive(false);
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm.currentOre == this) psm.currentOre = null;
        }
    }
}
