using UnityEngine;
using System.Collections; // Required for IEnumerator

public class OreCollector : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode mineKey = KeyCode.E; // Changed from chopKey to mineKey
    public float mineDistance = 0.1f;    // Changed from chopDistance to mineDistance
    public LayerMask oreLayer;         // Changed from treeLayer to oreLayer

    [Header("Item Data")]
    public Sprite oreIcon; // Changed from woodIcon to oreIcon

    [Header("References")]
    public Animator animator;
    public GameObject pickaxeObject;    // Changed from axeObject to pickaxeObject
    public GameObject PlayerWeapon;
    public CharacterMovement movement; // Script to control player movement

    private bool isMining = false; // Changed from isChopping to isMining

    void Update()
    {
        if (isMining) return; // Use isMining

        if (Input.GetKeyDown(mineKey)) // Use mineKey
        {
            TryMine(); // Changed from TryChop to TryMine
        }
    }

    void TryMine() // Changed from TryChop to TryMine
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, mineDistance, oreLayer)) // Use mineDistance, oreLayer
        {
            OreResource ore = hit.collider.GetComponent<OreResource>(); // Get OreResource
            if (ore != null)
            {
                StartCoroutine(MineSequence(ore)); // Changed from ChopSequence to MineSequence
            }
        }
    }

    public void PerformMine(OreResource ore) // Changed from PerformChop to PerformMine, TreeTarget to OreResource
    {
        // Call Hit on the ore resource
        ore.Hit(); // Changed from tree.Chop() to ore.Hit()

        // Add to Inventory and show Toast + Notification
        InventoryManager.Instance.AddItem("Ore", oreIcon, 1); // Changed item name and icon
    }

    // This method is called by the OreResource when the player interacts with it
    public IEnumerator StartMineFromExternal(OreResource ore) // Changed from StartChopFromExternal, TreeTarget to OreResource
    {
        isMining = true; // Use isMining

        // Disable movement and enable pickaxe
        movement.SetCanMove(false);
        PlayerWeapon.SetActive(false);
        pickaxeObject.SetActive(true); // Use pickaxeObject

        // Play animation
        if (animator != null)
            animator.SetTrigger("Mine"); // Use "Mine" animation trigger

        // Wait for animation to finish (e.g., 1.5 seconds)
        yield return new WaitForSeconds(1.5f);

        // Call the function on the ore
        PerformMine(ore);

        // Disable pickaxe and enable movement
        pickaxeObject.SetActive(false); // Use pickaxeObject
        PlayerWeapon.SetActive(true);
        movement.SetCanMove(true);
        isMining = false; // Use isMining
    }

    // This method is for direct interaction (e.g., raycast from player)
    System.Collections.IEnumerator MineSequence(OreResource ore) // Changed from ChopSequence, TreeTarget to OreResource
    {
        isMining = true; // Use isMining

        // Disable movement and enable pickaxe
        movement.SetCanMove(false);
        PlayerWeapon.SetActive(false);
        pickaxeObject.SetActive(true); // Use pickaxeObject

        // Play animation
        if (animator != null)
            animator.SetTrigger("Mine"); // Use "Mine" animation trigger

        // Wait for animation to finish (e.g., 1.5 seconds)
        yield return new WaitForSeconds(1.5f);

        // Call the function on the ore
        PerformMine(ore);

        // Disable pickaxe and enable movement
        pickaxeObject.SetActive(false); // Use pickaxeObject
        PlayerWeapon.SetActive(true);
        movement.SetCanMove(true);
        isMining = false; // Use isMining
    }
}