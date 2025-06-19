using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton;
    public Button declineButton;

    private string[] dialogueLines;
    private int currentLine;
    private System.Action onAccept;
    private System.Action onDecline;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines, System.Action acceptCallback, System.Action declineCallback)
    {
        dialogueLines = lines;
        currentLine = 0;
        onAccept = acceptCallback;
        onDecline = declineCallback;

        dialoguePanel.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        ShowLine();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CharacterMovement.Instance.SetCanMove(false); // ต้องมี script คุม movement
    }

    void ShowLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            // แสดงปุ่ม Accept / Decline
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
        }
    }

    public void Accept()
    {
        onAccept?.Invoke();
        EndDialogue();
    }

    public void Decline()
    {
        onDecline?.Invoke();
        EndDialogue();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterMovement.Instance.SetCanMove(true);
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }
}
