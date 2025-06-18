using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private List<Quest> allQuests = new List<Quest>();
    private Quest activeQuest;

    void Awake() {
        Instance = this;
    }

    void Update() {
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
            Debug.Log("Quest already active!");
            return;
        }

        activeQuest = quest;
        quest.StartQuest();
    }

    public void CompleteActiveQuest()
    {
        if (activeQuest != null)
        {
            activeQuest.CompleteQuest();
            activeQuest = null;
        }
    }
}
