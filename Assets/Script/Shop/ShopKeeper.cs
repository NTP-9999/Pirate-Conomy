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
    [Tooltip("How close before the UI goes 'press to open'")]
    private float interactableRange => maxDistance * 0.75f;
    public float maxDistance;
   

    private PlayerController playerController;
    private PlayerSkillController playerSkillController;
    private PlayerStateMachine playerStateMachine;
    public GameObject playerHUD; // ← new

    void Awake()
    {
        playerHUD = GameObject.Find("PlayerHUD");
        // cache player scripts
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerController == null) playerController = player.GetComponent<PlayerController>();
            if (playerSkillController == null) playerSkillController = player.GetComponent<PlayerSkillController>();
            if (playerStateMachine == null) playerStateMachine = player.GetComponent<PlayerStateMachine>();
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
        playerHUD.SetActive(false); // ซ่อน HUD ของผู้เล่น
        if (shopUIPanel != null) shopUIPanel.SetActive(true);
        shopManager?.OpenShop();
        PlayerAudioManager.Instance.PlayOneShot(PlayerAudioManager.Instance.opencloseShopSound);

        if (playerController != null) { playerController.enabled = false; playerController.canMove = false; }
        if (playerSkillController != null) playerSkillController.enabled = false;
        if (playerStateMachine    != null) playerStateMachine.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Time.timeScale   = 0f;
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        if (shopUIPanel != null) shopUIPanel.SetActive(false);
        PlayerAudioManager.Instance.PlayOneShot(PlayerAudioManager.Instance.opencloseShopSound);
        
        playerHUD.SetActive(true); // แสดง HUD ของผู้เล่น

        if (playerController != null) { playerController.enabled = true; playerController.canMove = true; }
        if (playerSkillController != null) playerSkillController.enabled = true;
        if (playerStateMachine    != null) playerStateMachine.enabled = true;

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
        if (!canOpenShop) return;

        playerInRange = true;
        if (other.CompareTag("Player"))
        {
            if (maxDistance == 0)
            {
                maxDistance = //Distance from ship to player
                    Vector3.Distance(interactPoint.transform.position, other.transform.position);
            }
            interactUI = InteractableUIManager.Instance.CreateResourceInteractUI(interactPoint).GetComponent<ResourceInteractUI>();
            interactUI.SetUp("Talk");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!canOpenShop) return;
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
        if (!canOpenShop) return;
        playerInRange = false;
        if (interactUI != null)
        {
            interactUI.HideUI();
            interactUI = null;
        }
    }
}
