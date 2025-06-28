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
    public Transform quest2Target; // เป้าหมายของเควส "ส่งจดหมาย" (อาจจะเป็นหมู่บ้านหรือ NPC คนอื่น)

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
        // ... (โค้ดส่วน Start() เดิมสำหรับการหา Reference ของ Player และ Camera) ...
        if (playerMovement == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerMovement = playerObj.GetComponent<CharacterMovement>();
            }
            if (playerMovement == null)
            {
                Debug.LogError("QuestGiver: CharacterMovement reference is missing! Please assign it in the Inspector or ensure Player has CharacterMovement.", this);
            }
        }

        if (playerCameraController == null)
        {
            if (playerMovement != null)
            {
                playerCameraController = playerMovement.GetComponentInChildren<FirstPersonCamera>();
            }
            if (playerCameraController == null)
            {
                playerCameraController = FindObjectOfType<FirstPersonCamera>();
            }
            if (playerCameraController == null)
            {
                Debug.LogError("QuestGiver: FirstPersonCamera script reference is missing! Please assign it in the Inspector or ensure it's in the scene.", this);
            }
        }

        if (pressEUI != null)
        {
            pressEUI.SetActive(false);
        }
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

                if (playerCameraController != null)
                {
                    playerCameraController.StartCameraElevation(cameraElevateOffset, cameraElevateSpeed);
                }

                DialogueManager.Instance.StartDialogue(dialogueLines, OnAccept, OnDecline);
                pressEUI.SetActive(false); 
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
        var nextQuest = new TalkToNPCQuest(quest2Name, quest2Description, quest2Target); // ใช้ TalkToNPCQuest หรือสร้างคลาสเฉพาะสำหรับเควสเดินทาง
        QuestManager.Instance.AddQuest(nextQuest);
        QuestManager.Instance.StartQuest(nextQuest); // นี่จะทำให้ UI เควสแสดงชื่อและรายละเอียดเควสใหม่ และ Waypoint ชี้ไปที่ quest2Target
        Debug.Log($"เริ่มเควสถัดไปแล้ว: {quest2Name}");

        nextQuestStarted = true; // ตั้ง Flag ว่าเควสถัดไปเริ่มแล้ว ป้องกันการเริ่มซ้ำ
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
}