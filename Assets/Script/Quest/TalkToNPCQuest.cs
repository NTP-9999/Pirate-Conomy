using UnityEngine;

public class TalkToNPCQuest : Quest
{
    public float questFinishRange;
    public TalkToNPCQuest(string name, string desc, Transform target, float finishRange = 5f)
        : base(name, desc, target, finishRange) { }

    public override void UpdateQuest()
    {
        if (!isActive || isCompleted) return;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null) return;

        if (target != null)
        {
            float distance = Vector3.Distance(Player.transform.position, target.position);
            if (distance < questFinishRange)
            {
                Debug.Log($"ถึงเป้าหมาย {target.name} แล้ว!");
                CompleteQuest();
            }
        }
    }

    public override void StartQuest()
    {
        base.StartQuest();
        Debug.Log($"[TalkToNPCQuest] StartQuest called, target: {target?.name}");
        WaypointUI.Instance.SetTarget(target, this);
    }
}