using UnityEngine;

public class IslandQuest : Quest
{
    public IslandQuest(string name, string desc, Transform target, float finishRange = 15f)
        : base(name, desc, target, finishRange) { }

    public override void UpdateQuest()
    {
        // ตรวจสอบว่าเควสยัง Active และยังไม่เสร็จสิ้น
        if (!isActive || isCompleted) return;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player GameObject not found for IslandQuest UpdateQuest!");
            return;
        }

        float distance = Vector3.Distance(Player.transform.position, target.position);
        if (distance < questFinishRange) // ใช้ระยะที่กำหนดใน questFinishRange
        {
            Debug.Log($"เควสเกาะ '{questName}' สำเร็จแล้ว! ถึงเกาะแล้ว (ระยะ {distance:F2}m)");
            CompleteQuest(); // ทำให้ UI เควสหายไป
        }
    }
} 