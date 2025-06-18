using UnityEngine;

public class WaypointUI : MonoBehaviour
{
    public static WaypointUI Instance;

    public Transform target;
    public Camera cam;
    public RectTransform icon;
    public TMPro.TextMeshProUGUI distanceText;
    public Quest questRef;

    void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform t, Quest quest)
    {
        target = t;
        questRef = quest;
        icon.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(true);
    }

    public void Clear()
    {
        target = null;
        questRef = null;
        icon.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);
        icon.position = screenPos + new Vector3(0, 20, 0);

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector3.Distance(Player.transform.position, target.position);
        distanceText.text = Mathf.RoundToInt(dist) + " m";
    }
}