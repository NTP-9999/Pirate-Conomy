using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public string[] dialogueLines = new string[] {
        "สวัสดี นักผจญภัย!",
        "ช่วยไปส่งจดหมายที่หมู่บ้านใกล้ ๆ ให้หน่อยนะ"
    };

    [Header("ข้อมูลเควสที่ 2 (หลังคุยกับ NPC)")]
    public string quest2Name = "ส่งจดหมาย";
    public string quest2Description = "เดินทางไปยังหมู่บ้านเพื่อส่งจดหมาย";
    public Transform quest2Target; // เป้าหมายของเควส "ส่งจดหมาย"

    [Header("Quest 2 Settings")]
    public float quest2FinishRange = 10f; 

    private bool playerInRange = false;
    private bool nextQuestStarted = false; 

    public GameObject pressEUI;

    // เปลี่ยนจาก CharacterMovement → PlayerController
    public PlayerController playerController; 
    public FirstPersonCamera playerCameraController; 
    
    public float cameraElevateOffset = 1.5f; 
    public float cameraElevateSpeed = 0.5f;
    public ShopKeeper shopKeeper;

    void Start()
    {
        // หา Player GameObject ที่ inactive ด้วย
        GameObject playerObj = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allObjects)
        {
            if (go.CompareTag("Player") && go.hideFlags == HideFlags.None)
            {
                playerObj = go;
                break;
            }
        }

        if (playerObj != null)
        {
            // หา PlayerController แทน
            if (playerController == null)
                playerController = playerObj.GetComponent<PlayerController>();

            if (playerCameraController == null)
                playerCameraController = playerObj.GetComponentInChildren<FirstPersonCamera>(true);
        }
        else
        {
            Debug.LogWarning("QuestGiver: ไม่พบ Player (แม้จะ inactive)");
        }

        if (pressEUI == null)
            pressEUI = GameObject.Find("PressEPrompt");
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!nextQuestStarted)
            {
                // ✏ ยังไม่เคยเริ่มเควสถัดไป → เปิด dialogue
                if (playerController != null)
                {
                    // ปิดการเคลื่อนที่
                    playerController.canMove = false;
                }
                DialogueManager.Instance.StartDialogue(dialogueLines, OnAccept, OnDecline);
                pressEUI.SetActive(false);

                if (playerCameraController != null)
                {
                    playerCameraController.StartCameraElevation(cameraElevateOffset, cameraElevateSpeed);
                }
            }
            else
            {
                // ✅ ถ้าเควสถัดไปเริ่มแล้ว → เปิดร้านค้าแทน
                if (shopKeeper != null)
                {
                    shopKeeper.OpenShop();
                    pressEUI.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEUI != null)
                pressEUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pressEUI.SetActive(false);

            // ถ้ายังไม่ได้เริ่มเควสถัดไป ให้คืนค่าสถานะการเคลื่อนที่และกล้อง
            if (!nextQuestStarted)
            {
                if (playerController != null)
                {
                    playerController.canMove = true;
                }
                if (playerCameraController != null)
                {
                    playerCameraController.ResetCameraElevation(cameraElevateSpeed);
                }
            }
        }
    }

    void OnAccept()
    {
        Debug.Log("Dialogue Accepted! กำลังเริ่มเควสถัดไป...");

        if (quest2Target == null)
        {
            Debug.LogError("QuestGiver: ไม่มี target สำหรับเควสที่ 2!");
            ResetPlayerAndCamera();
            return;
        }

        var nextQuest = new TalkToNPCQuest(
            quest2Name, quest2Description, quest2Target, quest2FinishRange
        );

        QuestManager.Instance.AddQuest(nextQuest);
        QuestManager.Instance.StartQuest(nextQuest);

        nextQuestStarted = true;
        if (shopKeeper != null)
            shopKeeper.EnableShop();

        ResetPlayerAndCamera();
        if (playerInRange && pressEUI != null)
            pressEUI.SetActive(true);
    }

    void OnDecline()
    {
        Debug.Log("Dialogue Declined. ผู้เล่นต้อง Accept เพื่อทำเควสต่อ.");
        ResetPlayerAndCamera();
    }

    void ResetPlayerAndCamera()
    {
        if (playerController != null)
        {
            playerController.canMove = true;
        }
        if (playerCameraController != null)
        {
            playerCameraController.ResetCameraElevation(cameraElevateSpeed);
        }
    }

    void OnDrawGizmos()
    {
        if (quest2Target != null)
        {
            DrawQuestGizmo(
                quest2Target.position,
                quest2FinishRange,
                quest2Name,
                nextQuestStarted ? true : false,
                nextQuestStarted ? Color.blue : Color.yellow
            );
        }
    }

    private void DrawQuestGizmo(
        Vector3 position, float range, string label, bool started, Color gizmoColor
    )
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(position, range);
        Gizmos.DrawLine(position, position + Vector3.right * range);

        #if UNITY_EDITOR
        Vector3 labelPosition = position + Vector3.up * (range + 1f);
        UnityEditor.Handles.Label(labelPosition,
            $"Quest: {label}\nRange: {range}m\nStarted: {started}");
        #endif
    }
}
