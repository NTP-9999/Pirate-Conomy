using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void AssignReferences()
    {
        // หา DialoguePanel แม้จะ inactive
        if (dialoguePanel == null)
        {
            var allPanels = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in allPanels)
            {
                if (go.name == "DialoguePanel")
                {
                    dialoguePanel = go;
                    break;
                }
            }
        }
        if (dialogueText == null)
        {
            var allTexts = Resources.FindObjectsOfTypeAll<TMPro.TextMeshProUGUI>();
            foreach (var t in allTexts)
            {
                if (t.name == "DialogueText")
                {
                    dialogueText = t;
                    break;
                }
            }
        }
        if (acceptButton == null)
        {
            var allButtons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var b in allButtons)
            {
                if (b.name == "AcceptButton")
                {
                    acceptButton = b;
                    break;
                }
            }
        }
        if (declineButton == null)
        {
            var allButtons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var b in allButtons)
            {
                if (b.name == "DeclineButton")
                {
                    declineButton = b;
                    break;
                }
            }
        }
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterMovement.Instance.SetCanMove(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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
