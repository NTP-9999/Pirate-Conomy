using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public string questName = "ไปหา NPC A";
    public string description = "คุยกับ NPC ที่หมู่บ้าน";
    public Transform target;

    private bool questGiven = false;

    void OnTriggerEnter(Collider other)
    {
        if (!questGiven && other.CompareTag("Player"))
        {
            var quest = new TalkToNPCQuest(questName, description, target);
            QuestManager.Instance.AddQuest(quest);
            QuestManager.Instance.StartQuest(quest);
            questGiven = true;
        }
    }
}
