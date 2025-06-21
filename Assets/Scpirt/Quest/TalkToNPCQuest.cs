using UnityEngine;

public class TalkToNPCQuest : Quest
{
    public TalkToNPCQuest(string name, string desc, Transform target)
        : base(name, desc, target) { }

    public override void UpdateQuest()
    {
        // ตรวจสอบว่าเควสยัง Active และยังไม่เสร็จสิ้น
        if (!isActive || isCompleted) return;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player GameObject not found for TalkToNPCQuest UpdateQuest!");
            return;
        }

        float distance = Vector3.Distance(Player.transform.position, target.position);
        if (distance < 2f) // กำหนดระยะที่ถือว่า "ถึงจุด"
        {
            Debug.Log($"เควส '{questName}' สำเร็จแล้วจากการเข้าใกล้! (ระยะ {distance:F2}m)");
            CompleteQuest(); // ทำให้ UI เควสหายไป
        }
    }
}