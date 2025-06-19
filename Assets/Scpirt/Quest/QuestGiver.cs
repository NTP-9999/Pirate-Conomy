using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public string[] dialogueLines = new string[] {
        "สวัสดี นักผจญภัย!",
        "ช่วยไปส่งจดหมายที่หมู่บ้านใกล้ ๆ ให้หน่อยนะ"
    };

    public string questName = "ส่งจดหมาย";
    public string description = "เดินไปส่งจดหมายที่หมู่บ้าน";
    public Transform target;

    private bool playerInRange = false;
    private bool questGiven = false;

    public GameObject pressEUI;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !questGiven)
        {
            DialogueManager.Instance.StartDialogue(dialogueLines, OnAccept, OnDecline);
            pressEUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pressEUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pressEUI.SetActive(false);
        }
    }

    void OnAccept()
    {
        var quest = new TalkToNPCQuest(questName, description, target);
        QuestManager.Instance.AddQuest(quest);
        QuestManager.Instance.StartQuest(quest);
        questGiven = true;
    }

    void OnDecline()
    {
        Debug.Log("ปฏิเสธเควส");
    }
}
