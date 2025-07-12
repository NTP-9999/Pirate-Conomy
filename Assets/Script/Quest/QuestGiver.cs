// Assets\Scpirt\Quest\QuestGiver.cs
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public string[] dialogueLines = new string[] {
        "สวัสดี นักผจญภัย!",
        "ช่วยไปส่งจดหมายที่หมู่บ้านใกล้ ๆ ให้หน่อยนะ"
    };

    // ** ไม่ต้องมี Initial Quest Info ที่นี่อีกแล้ว **
    // เพราะเควส "คุยกับ NPC" จะถูกสร้างและ Start โดย Logic การเริ่มเกม
    // และจบลงด้วย TalkToNPCQuest.UpdateQuest() เมื่อผู้เล่นเดินเข้าใกล้

    // ข้อมูลสำหรับเควส *ถัดไป* หลังจากผู้เล่นกด Accept Dialogue
    // นี่คือเควส "ส่งจดหมาย" ของคุณ
    [Header("ข้อมูลเควสที่ 2 (หลังคุยกับ NPC)")]
    public string quest2Name = "ส่งจดหมาย";
    public string quest2Description = "เดินทางไปยังหมู่บ้านเพื่อส่งจดหมาย";
    public Transform quest2Target; // เป้าหมายของเควส "ส่งจดหมาย"
    
    [Header("Quest 2 Settings")]
    public float quest2FinishRange = 10f; // ปรับได้ใน Inspector

    private bool playerInRange = false;
    private bool nextQuestStarted = false; // Flag เพื่อบอกว่าเควสถัดไป (ส่งจดหมาย) เริ่มต้นแล้ว

    public GameObject pressEUI;

    public CharacterMovement playerMovement; 
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

        // หา Component ใน Player
        if (playerObj != null)
        {
            if (playerMovement == null)
                playerMovement = playerObj.GetComponent<CharacterMovement>();

            if (playerCameraController == null)
                playerCameraController = playerObj.GetComponentInChildren<FirstPersonCamera>(true); // true = รวม inactive
        }
        else
        {
            Debug.LogWarning("QuestGiver: ไม่พบ Player (แม้จะ inactive)");
        }

        // หา PressEUI ตามเดิม
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
                if (playerMovement != null)
                {
                    playerMovement.SetCanMove(false); 
                    playerMovement.LockYPosition();   
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
                    pressEUI.SetActive(false); // ซ่อนไว้ตอนเปิดร้าน
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // แสดง UI "กด E" เฉพาะเมื่อเควสถัดไปยังไม่เคยถูกเริ่ม
            if (pressEUI != null)
            {
                pressEUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pressEUI.SetActive(false);
            if (!nextQuestStarted)
            {
                if (playerMovement != null)
                {
                    playerMovement.UnlockYPosition();
                    playerMovement.SetCanMove(true);
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

        Quest nextQuest;
        if (quest2Target != null)
        {
            nextQuest = new TalkToNPCQuest(quest2Name, quest2Description, quest2Target, quest2FinishRange);
            Debug.Log($"เริ่มเควสจุดเดียว: {quest2Name}");
        }
        else
        {
            Debug.LogError("QuestGiver: ไม่มี target สำหรับเควสที่ 2!");
            ResetPlayerAndCamera();
            return;
        }

        QuestManager.Instance.AddQuest(nextQuest);
        QuestManager.Instance.StartQuest(nextQuest);

        nextQuestStarted = true;
        if (shopKeeper != null)
        {
            shopKeeper.EnableShop();
        }
        ResetPlayerAndCamera();
        if (playerInRange && pressEUI != null)
        {
            pressEUI.SetActive(true);
        }
    }
    
    void OnDecline()
    {
        Debug.Log("Dialogue Declined. ผู้เล่นต้อง Accept เพื่อทำเควสต่อ.");
        ResetPlayerAndCamera();
        // ไม่ต้องเปลี่ยน nextQuestStarted เป็น true ที่นี่ เพราะยังไม่ได้เริ่มเควสถัดไป
    }

    void ResetPlayerAndCamera()
    {
        if (playerMovement != null)
        {
            playerMovement.UnlockYPosition();
            playerMovement.SetCanMove(true);
        }
        if (playerCameraController != null)
        {
            playerCameraController.ResetCameraElevation(cameraElevateSpeed);
        }
    }
    
    // Gizmos สำหรับแสดงระยะ QuestFinishRange ใน Scene View
    void OnDrawGizmos()
    {
        // แสดง Gizmo สำหรับ single target
        if (quest2Target != null)
        {
            DrawQuestGizmo(quest2Target.position, quest2FinishRange, 
                          quest2Name, nextQuestStarted, nextQuestStarted ? Color.blue : Color.yellow);
        }
        
        // แสดง Gizmo สำหรับ multiple targets
        // ลบส่วนนี้เนื่องจากไม่มีการสนับสนุน multiple targets แล้ว
    }

    private void DrawQuestGizmo(Vector3 position, float range, string label, bool started, Color gizmoColor)
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(position, range);
        
        // วาดเส้นจาก target ไปยังระยะ
        Gizmos.DrawLine(position, position + Vector3.right * range);
        
        // แสดงข้อความ
        #if UNITY_EDITOR
        Vector3 labelPosition = position + Vector3.up * (range + 1f);
        UnityEditor.Handles.Label(labelPosition, 
            $"Quest: {label}\nRange: {range}m\nStarted: {started}");
        #endif
    }
}