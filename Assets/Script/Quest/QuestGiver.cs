using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestGiver : MonoBehaviour
{
    [Header("Dialogue")]
    public string[] dialogueLines = {
        "สวัสดี นักผจญภัย!",
        "ช่วยไปส่งจดหมายที่หมู่บ้านใกล้ ๆ ให้หน่อยนะ"
    };

    [Header("Next Quest")]
    public string   quest2Name        = "ส่งจดหมาย";
    public string   quest2Description = "เดินทางไปยังหมู่บ้านเพื่อส่งจดหมาย";
    public Transform quest2Target;
    public float    quest2FinishRange = 10f;

    [Header("UI")]
    public Transform interactPoint;
    private ResourceInteractUI interactUI;
    private bool playerInRange = false;

    [Header("Ranges")]
    private float interactableRange => maxDistance * 0.75f;

    [Header("References")]
    public ShopKeeper shopKeeper;      // to open shop after quest
    private PlayerController      playerController;
    private FirstPersonCamera     playerCameraController;

    private bool nextQuestStarted = false;
    public float maxDistance = 0;

    void Awake()
    {
        // find player & camera if not set
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerController      == null) playerController      = player.GetComponent<PlayerController>();
            if (playerCameraController == null) playerCameraController = player.GetComponentInChildren<FirstPersonCamera>(true);
        }

        if (interactPoint == null) interactPoint = transform;
    }

    void Update()
    {
        if (!playerInRange || interactUI == null) return;

        // press E to talk or to open shop
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!nextQuestStarted)
            {
                // start dialogue
                playerController.canMove = false;
                DialogueManager.Instance.StartDialogue(dialogueLines, OnAccept, OnDecline);
                interactUI.HideUI();

                // optionally elevate camera
                playerCameraController?.StartCameraElevation(100f, 14f);
            }
            else
            {
                // quest done → open shop
                shopKeeper.OpenShop();
                interactUI.HideUI();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
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
        playerInRange = false;
        if (other.CompareTag("Player"))
        {
            interactUI.HideUI();
        }

        // restore movement/camera if mid-dialogue
        if (!nextQuestStarted)
        {
            playerController.canMove = true;
            playerCameraController?.ResetCameraElevation(0.5f);
        }
    }

    void OnAccept()
    {
        // start the next quest
        var nextQuest = new TalkToNPCQuest(
            quest2Name, quest2Description, quest2Target, quest2FinishRange
        );
        QuestManager.Instance.AddQuest(nextQuest);
        QuestManager.Instance.StartQuest(nextQuest);

        nextQuestStarted = true;
        playerCameraController?.ResetCameraElevation(20f);
    }

    void OnDecline()
    {
        // player declined
        playerController.canMove = true;
        playerCameraController?.ResetCameraElevation(20f);
    }
}
