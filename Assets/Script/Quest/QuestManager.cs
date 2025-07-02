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
            var initialQuest = new TalkToNPCQuest("คุยกับ NPC", "เดินทางไปหา NPC เพื่อเริ่มต้นภารกิจ", initialQuestGiverTarget);
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
}