using UnityEngine;

public class TalkToNPCQuest : Quest
{
    public float questFinishRange;
    public TalkToNPCQuest(string name, string desc, Transform target, float finishRange = 5f)
        : base(name, desc, target, finishRange) { }

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
        if (distance < questFinishRange) // ใช้ระยะที่กำหนดใน questFinishRange
        {
            Debug.Log($"เควส '{questName}' สำเร็จแล้วจากการเข้าใกล้! (ระยะ {distance:F2}m)");
            CompleteQuest(); // ทำให้ UI เควสหายไป
        }
    }
}