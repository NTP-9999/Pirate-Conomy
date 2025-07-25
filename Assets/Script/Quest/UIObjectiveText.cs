using UnityEngine;
using TMPro;

public class UIObjectiveText : MonoBehaviour
{
    public static UIObjectiveText Instance;

    [Header("Quest Text")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;

    void Awake()
    {
        Instance = this;
    }

    public void SetText(string questName, string description)
    {
        if (questNameText != null)
            questNameText.text = questName;

        if (descriptionText != null)
            descriptionText.text = "ห  " + description;
    }

    public void Clear()
    {
        if (questNameText != null)
            questNameText.text = "";

        if (descriptionText != null)
            descriptionText.text = "";
    }
}
