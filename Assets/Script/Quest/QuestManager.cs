// Assets\Scpirt\Quest\QuestManager.cs
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

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
            // ทำให้ QuestManager ไม่ถูกทำลายเมื่อเปลี่ยน Scene (ถ้าคุณต้องการ)
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // *** NEW: เริ่มเควสแรกสุด "คุยกับ NPC" เมื่อเกมเริ่ม ***
        // ตรวจสอบว่ามี target สำหรับเควสเริ่มต้น และยังไม่มีเควส active
        if (initialQuestGiverTarget != null && activeQuest == null)
        {
            var initialQuest = new TalkToNPCQuest("คุยกับ NPC", "เดินทางไปหา NPC เพื่อเริ่มต้นภารกิจ", initialQuestGiverTarget, initialQuestFinishRange);
            AddQuest(initialQuest); // เพิ่มเควสเข้า List
            StartQuest(initialQuest); // เริ่มเควสนี้ให้เป็น Active Quest และแสดง UI
            Debug.Log("เควสเริ่มต้น: 'คุยกับ NPC' ถูกเริ่มแล้ว");
        }
        else if (initialQuestGiverTarget == null)
        {
            Debug.LogError("QuestManager: กรุณาตั้งค่า 'Initial Quest Giver Target' ใน Inspector!");
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
        allQuests.Add(quest);
        if (autoStart)
        {
            StartQuest(quest);
        }
    }

    public void StartQuest(Quest quest)
    {
        if (activeQuest != null && !activeQuest.isCompleted)
        {
            // อาจจะต้องการเปลี่ยนเควสไปเลย แทนที่จะ Debug.Log
            // หรือตรวจสอบว่าเป็นเควสเดียวกันหรือไม่
            Debug.Log($"มีเควสกำลัง Active อยู่แล้ว ({activeQuest.questName}). ไม่สามารถเริ่มเควส '{quest.questName}' ได้");
            // ถ้าคุณต้องการให้มันบังคับเปลี่ยนเควสเลย uncomment บรรทัดนี้
            // CompleteActiveQuest(); 
        }

        activeQuest = quest;
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
}