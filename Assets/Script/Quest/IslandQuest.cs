// Assets\Scpirt\Quest\IslandQuest.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandQuest : MonoBehaviour
{
    public string questName = "";
    public string questDesc = "";
    public Transform questTarget;
    public float questFinishRange = 10f;

    void Start()
    {
        Debug.Log($"[IslandQuest] Starting quest: {questName}");
        
        // ตรวจสอบว่ามี questTarget หรือไม่
        if (questTarget == null)
        {
            Debug.LogError($"[IslandQuest] Quest target is null for quest: {questName}");
            return;
        }

        // สร้าง Quest ใหม่
        Quest quest = new TalkToNPCQuest(questName, questDesc, questTarget, questFinishRange);
        Debug.Log($"[IslandQuest] Created quest: {questName} with target: {questTarget.name}");
        
        // ส่งให้ QuestManager จัดการ
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddIslandQuest(quest, autoStart: true);
            Debug.Log($"[IslandQuest] Quest {questName} added to QuestManager and started");
        }
        else
        {
            Debug.LogError("[IslandQuest] QuestManager.Instance is null!");
        }
    }

    // --- Gizmo Drawing ---
    void OnDrawGizmos()
    {
        if (questTarget != null)
        {
            DrawQuestGizmo(questTarget.position, questFinishRange, questName, true, Color.green);
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
        UnityEditor.Handles.Label(labelPosition, $"Quest: {questName}\nRange: {range}m\nActive: {isActive}");
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