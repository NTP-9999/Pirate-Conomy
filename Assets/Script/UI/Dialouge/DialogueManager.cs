using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool IsInDialogue { get; private set; }
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton;
    public Button declineButton;

    private string[] dialogueLines;
    private int currentLine;
    private System.Action onAccept;
    private System.Action onDecline;

    private PlayerController playerController;

    private void Awake()
    {
        Debug.Log("DialogueManager Awake called!");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AssignReferences();
        FindPlayerController();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("DialogueManager: Scene loaded, re-assigning references.");
        AssignReferences();
        FindPlayerController();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void AssignReferences()
    {
        // หา DialoguePanel แม้จะ inactive
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("DialoguePanel");
        }
        if (dialogueText == null)
        {
            dialogueText = GameObject.Find("DialogueText")?.GetComponent<TextMeshProUGUI>();
        }
        if (acceptButton == null)
        {
            acceptButton = GameObject.Find("AcceptButton")?.GetComponent<Button>();
        }
        if (declineButton == null)
        {
            declineButton = GameObject.Find("DeclineButton")?.GetComponent<Button>();
        }
    }
    private void FindPlayerController()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
                Debug.LogWarning("DialogueManager: ไม่พบ PlayerController บน Player");
        }
        else
        {
            Debug.LogWarning("DialogueManager: ไม่พบ GameObject Tag \"Player\"");
        }
    }

    public void StartDialogue(string[] lines, System.Action acceptCallback, System.Action declineCallback)
    {
        IsInDialogue = true;
        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.enabled = false;
        }
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

        // ✂ ตรงนี้เปลี่ยนเป็น PlayerController
        if (playerController != null)
            playerController.canMove = false;

        var player = GameObject.FindGameObjectWithTag("Player");
        HideAllChildrenExceptCamera(player);
    }
    void HideAllChildrenExceptCamera(GameObject player)
    {
        foreach (Transform child in player.transform)
        {
            if (child.GetComponentInChildren<Camera>() != null)
                continue;

            child.gameObject.SetActive(false);
        }
    }

    void ShowAllChildren(GameObject player)
    {
        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(true);
        }
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

    private void EndDialogue()
    {
        IsInDialogue = false;
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.canMove = true;
        }
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✂ ตรงนี้ก็เช่นกัน
        if (playerController != null)
            playerController.canMove = true;

        var player = GameObject.FindGameObjectWithTag("Player");
        ShowAllChildren(player);
    }

    private void Update()
    {
        Debug.Log("DialogueManager Update running...");
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spacebar pressed, advancing dialogue...");
            NextLine();
        }
    }
}
