using UnityEngine;

public class DialogueButtonHandler : MonoBehaviour
{
    // เรียกใช้ใน Inspector หรือ UnityEvent ของปุ่ม Accept
    public void Accept()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.Accept();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance is null!");
        }
    }

    // เรียกใช้ใน Inspector หรือ UnityEvent ของปุ่ม Decline
    public void Decline()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.Decline();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance is null!");
        }
    }
} 