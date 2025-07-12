using UnityEngine;

public abstract class Quest
{
    public virtual string questName { get; protected set; }
    public virtual string description { get; protected set; }
    public bool isActive = false;
    public bool isCompleted = false;

    public Transform target; // Only single target supported
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
        if (WaypointUI.Instance != null)
            WaypointUI.Instance.SetTarget(target, this);
        if (UIObjectiveText.Instance != null)
            UIObjectiveText.Instance.SetText(questName + " " + description);
    }

    public virtual void CompleteQuest()
    {
        isCompleted = true;
        isActive = false;
        if (WaypointUI.Instance != null)
            WaypointUI.Instance.Clear();
        if (UIObjectiveText.Instance != null)
            UIObjectiveText.Instance.Clear();
    }

    public abstract void UpdateQuest(); // แต่ละเควสมี logic ไม่เหมือนกัน
}
