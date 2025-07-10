using UnityEngine;

public abstract class Quest
{
    public string questName;
    public string description;
    public bool isActive = false;
    public bool isCompleted = false;

    public Transform target; // จุดที่ Waypoint จะชี้
    public float questFinishRange = 5f; // ระยะที่เควสจะสำเร็จ (ปรับได้ใน Inspector)

    public Quest(string name, string desc, Transform target, float finishRange = 5f)
    {
        this.questName = name;
        this.description = desc;
        this.target = target;
        this.questFinishRange = finishRange;
    }

    public virtual void StartQuest()
    {
        isActive = true;
        WaypointUI.Instance.SetTarget(target, this); // ส่งเควสนี้ให้ Waypoint UI
        UIObjectiveText.Instance.SetText(questName + ": " + description);
    }

    public virtual void CompleteQuest()
    {
        isCompleted = true;
        isActive = false;
        WaypointUI.Instance.Clear();
        UIObjectiveText.Instance.Clear();
    }

    public abstract void UpdateQuest(); // แต่ละเควสมี logic ไม่เหมือนกัน
}
