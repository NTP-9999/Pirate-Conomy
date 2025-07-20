using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopKeeper : MonoBehaviour
{
    [Header("Shop Settings")]
    public GameObject shopUIPanel;  
    public ShopManager shopManager; 
    public bool shopAlwaysOpen = false;

    [Header("UI")]
    [Tooltip("Where the 'Press E' UI will appear")]
    public Transform interactPoint;
    private ResourceInteractUI interactUI;
    private bool playerInRange = false;
    private bool canOpenShop = false;              // ← new

    [Header("Ranges")]
    [Tooltip("How far away to show the UI at all")]
    public float showRange = 8f;
    [Tooltip("How close before the UI goes 'press to open'")]
    public float interactableRange = 5f;

    private PlayerController      playerController;
    private PlayerSkillController playerSkillController;

    void Awake()
    {
        // cache player scripts
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerController      == null) playerController      = player.GetComponent<PlayerController>();
            if (playerSkillController == null) playerSkillController = player.GetComponent<PlayerSkillController>();
        }

        if (interactPoint == null) interactPoint = transform;
        if (shopUIPanel  != null)  shopUIPanel.SetActive(false);

        // initialize canOpenShop
        canOpenShop = shopAlwaysOpen;
    }

    void Update()
    {
        if (!canOpenShop) return;

        if (playerInRange &&
            interactUI != null &&
            interactUI.interactUIState == InteractUIState.Interactable &&
            Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        if (shopUIPanel != null) shopUIPanel.SetActive(true);
        shopManager?.OpenShop();

        if (playerController      != null) { playerController.enabled = false; playerController.canMove = false; }
        if (playerSkillController != null) playerSkillController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Time.timeScale   = 0f;
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        if (shopUIPanel != null) shopUIPanel.SetActive(false);

        if (playerController      != null) { playerController.enabled = true;  playerController.canMove = true; }
        if (playerSkillController != null) playerSkillController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // ← the missing method!
    public void EnableShop()
    {
        canOpenShop = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canOpenShop || !other.CompareTag("Player")) return;
        playerInRange = true;
        interactUI = InteractableUIManager
                       .Instance
                       .CreateResourceInteractUI(interactPoint)
                       .GetComponent<ResourceInteractUI>();
        interactUI.SetUp("Talk");
    }

    void OnTriggerStay(Collider other)
    {
        if (!canOpenShop || !playerInRange || !other.CompareTag("Player") || interactUI == null)
            return;

        float dist = Vector3.Distance(other.transform.position, interactPoint.position);
        if (dist <= interactableRange)
            interactUI.Interactable();
        else if (dist <= showRange)
            interactUI.ReturnToShowInteractable();
        else
        {
            interactUI.HideUI();
            interactUI = null;
            playerInRange = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!canOpenShop || !other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactUI != null)
        {
            interactUI.HideUI();
            interactUI = null;
        }
    }
}
