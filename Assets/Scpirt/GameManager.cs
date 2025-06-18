using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform startingTarget = GameObject.Find("Target1").transform;
        var firstQuest = new TalkToNPCQuest("Tutorial", "Talk To Merchant", startingTarget);
        QuestManager.Instance.AddQuest(firstQuest, autoStart: true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
