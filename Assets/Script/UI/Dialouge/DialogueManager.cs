using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool IsInDialogue { get; private set; }

    // จะหาให้เองจากในฉาก
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton;
    public Button declineButton;

    private string[] dialogueLines;
    private int currentLine;
    private Action onAccept;
    private Action onDecline;
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // ลงทะเบียน callback เมื่อลากฉากใหม่เสร็จ
            SceneManager.sceneLoaded += OnSceneLoaded;

            // หา reference ครั้งแรก
            AssignReferences();
            FindPlayerController();
            HideDialogueUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ถูกเรียกอัตโนมัติเมื่อ Scene ใหม่ถูกโหลด
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignReferences();
        FindPlayerController();
        HideDialogueUI();
    }

    // ปิด Panel ก่อนเริ่ม
    private void HideDialogueUI()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    // หา GameObject และ Component ในฉาก โดยอาศัยชื่อ
    private void AssignReferences()
    {
        dialoguePanel   = null;
        dialogueText    = null;
        acceptButton    = null;
        declineButton   = null;

        var activeScene = SceneManager.GetActiveScene();
        var roots       = activeScene.GetRootGameObjects();

        // 1) หา DialoguePanel
        foreach (var go in roots)
        {
            var panelT = FindInChildren(go.transform, "DialoguePanel");
            if (panelT != null)
            {
                dialoguePanel = panelT.gameObject;
                break;
            }
        }

        if (dialoguePanel == null)
        {
            Debug.LogWarning($"[DialogueManager] ไม่พบ DialoguePanel ในฉาก \"{activeScene.name}\"");
            return;
        }

        // 2) หา DialogueText, AcceptButton, DeclineButton จากใต้ Panel
        var panelTransform = dialoguePanel.transform;

        var textT = FindInChildren(panelTransform, "DialogueText");
        if (textT != null)
            dialogueText = textT.GetComponent<TextMeshProUGUI>();

        var acceptT = FindInChildren(panelTransform, "AcceptButton");
        if (acceptT != null)
            acceptButton = acceptT.GetComponent<Button>();

        var declineT = FindInChildren(panelTransform, "DeclineButton");
        if (declineT != null)
            declineButton = declineT.GetComponent<Button>();
    }

    // ค้นหา Transform ชื่อตามต้องการ (รวม inactive ได้) ในบรรดา children ทั้งหมด
    private Transform FindInChildren(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            var result = FindInChildren(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    private void FindPlayerController()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
                Debug.LogWarning("[DialogueManager] ไม่พบ PlayerController บน GameObject 'Player'");
        }
    }

    public void StartDialogue(string[] lines, Action acceptCallback, Action declineCallback)
    {
        if (dialoguePanel == null) AssignReferences(); // กันพลาด
        if (dialoguePanel == null) return;

        IsInDialogue    = true;
        dialogueLines   = lines;
        currentLine     = 0;
        onAccept        = acceptCallback;
        onDecline       = declineCallback;

        dialoguePanel.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        ShowLine();

        // บล็อคการเคลื่อนที่ผู้เล่น
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.canMove = false;
        }

        // วาง Cursor
        Cursor.lockState  = CursorLockMode.None;
        Cursor.visible    = true;

        // ซ่อน model player ชั่วคราว
        HideAllChildrenExceptCamera(GameObject.FindGameObjectWithTag("Player"));
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
            ShowLine();
        else
        {
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
        dialoguePanel.SetActive(false);

        // คืนสิทธิ์ผู้เล่น
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.canMove = true;
        }

        Cursor.lockState  = CursorLockMode.Locked;
        Cursor.visible    = false;

        ShowAllChildren(GameObject.FindGameObjectWithTag("Player"));
    }

    private void ShowLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    private void HideAllChildrenExceptCamera(GameObject player)
    {
        if (player == null) return;
        foreach (Transform c in player.transform)
            if (c.GetComponentInChildren<Camera>() == null)
                c.gameObject.SetActive(false);
    }

    private void ShowAllChildren(GameObject player)
    {
        if (player == null) return;
        foreach (Transform c in player.transform)
            c.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
            NextLine();
    }
}
