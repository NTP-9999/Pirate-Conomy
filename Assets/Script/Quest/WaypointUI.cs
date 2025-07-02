using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointUI : MonoBehaviour
{
    public static WaypointUI Instance;

    public Transform target;
    public Camera cam;
    public RectTransform icon;
    public TextMeshProUGUI distanceText;
    public Quest questRef;

    public Vector2 screenPadding = new Vector2(50f, 50f);

    private Transform playerTransform;

    void Awake()
    {
        Instance = this;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("WaypointUI: Cannot find GameObject with tag 'Player'");
        }
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
        if (target == null || cam == null || playerTransform == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        bool isBehind = screenPos.z < 0;
        if (isBehind)
        {
            screenPos *= -1f;
        }

        Vector2 screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
        Vector2 screenPosition = new Vector2(screenPos.x, screenPos.y);
        Vector2 direction = (screenPosition - screenCenter).normalized;

        bool isOffscreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffscreen)
        {
            screenPosition = screenCenter + direction * Mathf.Min(screenCenter.x - screenPadding.x, screenCenter.y - screenPadding.y);
        }

        icon.position = screenPosition;
        distanceText.transform.position = new Vector3(screenPosition.x, screenPosition.y - 20f, 0);

        float dist = Vector3.Distance(playerTransform.position, target.position);
        distanceText.text = Mathf.RoundToInt(dist) + " m";

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        icon.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 เพื่อให้ปลายไอคอนชี้ขึ้น
    }
}
