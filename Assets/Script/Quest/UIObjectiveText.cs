using UnityEngine;

public class UIObjectiveText : MonoBehaviour
{
    public static UIObjectiveText Instance;
    public TMPro.TextMeshProUGUI text;

    void Awake() { Instance = this; }

    public void SetText(string t) { text.text = t; }
    public void Clear() { text.text = ""; }
}
