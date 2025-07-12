// Assets\Scpirt\Quest\QuestManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    // เพิ่ม static flag เพื่อป้องกันการสร้างเควสซ้ำ
    private static bool hasInitialQuestBeenCreated = false;

    private List<Quest> allQuests = new List<Quest>();
    public Quest activeQuest; // เปลี่ยนเป็น public เพื่อให้ QuestGiver เข้าถึง activeQuest ได้ง่ายขึ้น (สำหรับการ Debug)

    // *** NEW: Reference ไปยัง QuestGiver สำหรับเควสเริ่มต้น ***
    // ลาก GameObject ของ NPC ที่เป็น QuestGiver เข้ามาใน Inspector
    [Header("สำหรับเควสเริ่มต้นเกม")]
    public Transform initialQuestGiverTarget; // ลาก GameObject ของ NPC Quest Giver มาใส่
    [Range(1f, 1000f)]
    public float initialQuestFinishRange = 5f; // ระยะที่เควสเริ่มต้นจะสำเร็จ
    
    [Header("Quest Gizmo Settings")]
    public bool showQuestGizmos = true; // เปิด/ปิดการแสดง Gizmos
    public Color activeQuestColor = Color.green;
    public Color inactiveQuestColor = Color.yellow;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้ QuestManager ไม่ถูก destroy เมื่อเปลี่ยน scene
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Debug.Log("[QuestManager] QuestManager set to DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("[QuestManager] Another QuestManager found, destroying duplicate");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("[QuestManager] Start called");
        
        // สร้างเควสเริ่มต้นเฉพาะถ้ามี Target1 และยังไม่เคยสร้าง
        if (!hasInitialQuestBeenCreated) 
        {
            GameObject go = GameObject.Find("Target1");
            Debug.Log($"[QuestManager] Looking for Target1: {(go != null ? "Found" : "Not found")}");
            
            if (go != null)
            {
                Transform startingTarget = go.transform;
                Debug.Log($"[QuestManager] Creating initial quest with target: {startingTarget.name}");
                var firstQuest = new TalkToNPCQuest("Tutorial", "Talk To Merchant", startingTarget);
                AddQuest(firstQuest, autoStart: true);
                hasInitialQuestBeenCreated = true; // ตั้ง flag ว่าได้สร้างแล้ว
                Debug.Log("[QuestManager] Initial quest created and started");
            }
            else
            {
                Debug.LogWarning("QuestManager: ไม่พบ Target1 ใน scene");
            }
        }
        else
        {
            Debug.Log("[QuestManager] Initial quest already created, skipping");
        }
    }

    void Update()
    {
        // เช็คเฉพาะเควสเริ่มต้น (activeQuest ยังไม่ complete)
        if (activeQuest != null && !activeQuest.isCompleted && initialQuestGiverTarget != null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                float distance = Vector3.Distance(playerObj.transform.position, initialQuestGiverTarget.position);
                // Debug.Log($"[QuestManager] Player-Target Distance: {distance}, FinishRange: {initialQuestFinishRange}");
                if (distance < initialQuestFinishRange)
                {
                    Debug.Log("[QuestManager] Player entered quest range! Complete quest.");
                    CompleteActiveQuest();
                }
            }
        }

        // Logic เดิมสำหรับ quest อื่นๆ
        if (activeQuest != null && activeQuest.isActive && !activeQuest.isCompleted)
        {
            activeQuest.UpdateQuest();
        }
    }

    public void AddQuest(Quest quest, bool autoStart = false)
    {
        Debug.Log($"[QuestManager] AddQuest called - Quest: {quest?.questName}, AutoStart: {autoStart}");
        allQuests.Add(quest);
        if (autoStart)
        {
            StartQuest(quest);
        }
    }
    
    // Method สำหรับ IslandQuest เพื่อสร้างเควสโดยไม่ถูก block โดย static flag
    public void AddIslandQuest(Quest quest, bool autoStart = false)
    {
        Debug.Log($"[QuestManager] AddIslandQuest called - Quest: {quest?.questName}, AutoStart: {autoStart}");
        allQuests.Add(quest);
        if (autoStart)
        {
            StartQuest(quest);
        }
    }

    public void StartQuest(Quest quest)
    {
        Debug.Log($"[QuestManager] StartQuest called - Quest: {quest?.questName}");
        
        // ถ้ามีเควส active อยู่ ให้ complete ก่อน
        if (activeQuest != null && !activeQuest.isCompleted)
        {
            Debug.Log("[QuestManager] Completing previous active quest");
            CompleteActiveQuest();
        }

        activeQuest = quest;
        Debug.Log($"[QuestManager] Setting active quest: {quest?.questName}");
        quest.StartQuest(); // เรียก StartQuest ของเควสที่ทำให้ UI แสดง
    }

    public void CompleteActiveQuest()
    {
        if (activeQuest != null)
        {
            activeQuest.CompleteQuest(); // ทำให้ UI หายไป
            activeQuest = null;
        }
    }
    
    // Method สำหรับ reset static flag เมื่อเกมเริ่มใหม่
    public static void ResetStaticData()
    {
        hasInitialQuestBeenCreated = false;
    }
    
    // Gizmos สำหรับแสดงระยะ QuestFinishRange ใน Scene View
    void OnDrawGizmos()
    {
        if (!showQuestGizmos) return;
        
        // แสดง Gizmos ของ Active Quest
        if (activeQuest != null && activeQuest.target != null)
        {
            DrawQuestGizmo(activeQuest.target.position, activeQuest.questFinishRange, 
                          activeQuest.questName, true, activeQuestColor);
        }
        
        // แสดง Gizmos ของ Initial Quest Target (ถ้ามี)
        if (initialQuestGiverTarget != null)
        {
            DrawQuestGizmo(initialQuestGiverTarget.position, initialQuestFinishRange, 
                          "Initial Quest", false, inactiveQuestColor);
        }
    }
    
    void DrawQuestGizmo(Vector3 position, float range, string questName, bool isActive, Color color)
    {
        // วาดวงกลมแสดงระยะ QuestFinishRange
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, range);
        
        // วาดเส้นจาก target ไปยังระยะ
        Gizmos.DrawLine(position, position + Vector3.right * range);
        
        // วาดลูกศรชี้ไปยัง target
        DrawArrow(position, position + Vector3.up * 2f, color);
        
        // แสดงข้อความ
        #if UNITY_EDITOR
        Vector3 labelPosition = position + Vector3.up * (range + 1f);
        UnityEditor.Handles.Label(labelPosition, 
            $"Quest: {questName}\nRange: {range}m\nActive: {isActive}");
        #endif
    }
    
    void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);
        
        // วาดหัวลูกศร
        Vector3 direction = (end - start).normalized;
        Vector3 right = Vector3.Cross(direction, Vector3.forward).normalized;
        Vector3 arrowHead1 = end - direction * 0.5f + right * 0.3f;
        Vector3 arrowHead2 = end - direction * 0.5f - right * 0.3f;
        
        Gizmos.DrawLine(end, arrowHead1);
        Gizmos.DrawLine(end, arrowHead2);
    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("[QuestManager] Scene unloaded, completing active quest.");
        CompleteActiveQuest();
        
        // Reset static flag เพื่อให้ IslandQuest สามารถสร้างเควสใหม่ได้ใน scene ใหม่
        hasInitialQuestBeenCreated = false;
        Debug.Log("[QuestManager] Reset hasInitialQuestBeenCreated flag for new scene");
    }

    void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}