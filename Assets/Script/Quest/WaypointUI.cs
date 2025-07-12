using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Added for List

public class WaypointUI : MonoBehaviour
{
    public static WaypointUI Instance;

    [Header("References")]
    public Transform target;
    public Camera cam;
    public RectTransform icon;
    public TextMeshProUGUI distanceText;
    public Quest questRef; // Assuming 'Quest' is a defined class

    [Header("UI Settings")]
    public Vector2 screenPadding = new Vector2(50f, 50f);
    public float smoothSpeed = 10f; // Controls how fast the UI moves to its target

    private Transform playerTransform;

    void Awake()
    {  
        if (target == null)
        {
            target = GameObject.Find("Target1").transform;
        }
        GameObject playerObj = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allObjects)
        {
            if (go.CompareTag("Player") && go.hideFlags == HideFlags.None)
            {
                playerObj = go;
                break;
            }
        }
        if (playerObj != null)
        {
            if (cam == null)
            {
                cam = FindObjectOfType<Camera>();
            }
        }
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("WaypointUI: Cannot find GameObject with tag 'Player'. Make sure your player object has the 'Player' tag.");
        }

        // Initialize UI visibility
        Clear();
    }

    /// <summary>
    /// Sets a new target for the waypoint UI.
    /// </summary>
    /// <param name="t">The transform of the target object.</param>
    /// <param name="quest">The associated quest reference.</param>
    public void SetTarget(Transform t, Quest quest)
    {
        this.target = t;
        this.questRef = quest;
        if (icon != null) icon.gameObject.SetActive(true);
        if (distanceText != null) distanceText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Clears the current waypoint, hiding the UI elements.
    /// </summary>
    public void Clear()
    {
        target = null;
        questRef = null;
        if (icon != null) icon.gameObject.SetActive(false);
        if (distanceText != null) distanceText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the camera used for world-to-screen point conversion.
    /// </summary>
    /// <param name="newCam">The new camera to use.</param>
    public void SetCamera(Camera newCam)
    {
        cam = newCam;
    }

    /// <summary>
    /// Sets the player's transform for distance calculation.
    /// </summary>
    /// <param name="t">The player's transform.</param>
    public void SetReferenceTransform(Transform t)
    {
        playerTransform = t;
    }

    // Use LateUpdate for UI positioning to ensure it updates after all other movements
    void LateUpdate()
    {
        // Ensure all necessary references are set
        if (target == null || cam == null || playerTransform == null)
        {
            if (icon != null && icon.gameObject.activeSelf) icon.gameObject.SetActive(false);
            if (distanceText != null && distanceText.gameObject.activeSelf) distanceText.gameObject.SetActive(false);
            return;
        }

        // Convert world position to screen position
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        // Check if the target is in front of the camera (z > 0)
        bool isTargetInFrontOfCamera = screenPos.z > 0;

        // Check if the target is within the viewport (0 to Screen.width/height)
        bool isTargetOnScreen = isTargetInFrontOfCamera &&
                                screenPos.x > 0 && screenPos.x < Screen.width &&
                                screenPos.y > 0 && screenPos.y < Screen.height;

        if (isTargetOnScreen)
        {
            if (icon != null) icon.gameObject.SetActive(true);
            if (distanceText != null) distanceText.gameObject.SetActive(true);

            // Directly set position for on-screen targets
            if (icon != null) icon.position = Vector2.Lerp(icon.position, new Vector2(screenPos.x, screenPos.y), Time.deltaTime * smoothSpeed);
            if (distanceText != null) distanceText.transform.position = Vector3.Lerp(distanceText.transform.position, new Vector3(screenPos.x, screenPos.y - 20f, 0), Time.deltaTime * smoothSpeed);

            // No rotation needed for on-screen target (or set a default if desired)
            if (icon != null) icon.rotation = Quaternion.Lerp(icon.rotation, Quaternion.identity, Time.deltaTime * smoothSpeed); // Reset to no rotation
        }
        else // Target is off-screen
        {
            if (isTargetInFrontOfCamera)
            {
                if (icon != null) icon.gameObject.SetActive(true);
                if (distanceText != null) distanceText.gameObject.SetActive(true);

                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 clampedPosition = new Vector2(
                    Mathf.Clamp(screenPos.x, screenPadding.x, Screen.width - screenPadding.x),
                    Mathf.Clamp(screenPos.y, screenPadding.y, Screen.height - screenPadding.y)
                );

                if (screenPos.x <= screenPadding.x || screenPos.x >= Screen.width - screenPadding.x ||
                    screenPos.y <= screenPadding.y || screenPos.y >= Screen.height - screenPadding.y)
                {
                    Vector2 directionToTarget = (new Vector2(screenPos.x, screenPos.y) - screenCenter).normalized;
                    float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
                    if (icon != null) icon.rotation = Quaternion.Slerp(icon.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * smoothSpeed);
                    if (icon != null) icon.position = Vector2.Lerp(icon.position, clampedPosition, Time.deltaTime * smoothSpeed);
                    if (distanceText != null) distanceText.transform.position = Vector3.Lerp(distanceText.transform.position, new Vector3(clampedPosition.x, clampedPosition.y - 20f, 0), Time.deltaTime * smoothSpeed);
                }
                else
                {
                    if (icon != null) icon.position = Vector2.Lerp(icon.position, new Vector2(screenPos.x, screenPos.y), Time.deltaTime * smoothSpeed);
                    if (distanceText != null) distanceText.transform.position = Vector3.Lerp(distanceText.transform.position, new Vector3(screenPos.x, screenPos.y - 20f, 0), Time.deltaTime * smoothSpeed);
                    if (icon != null) icon.rotation = Quaternion.Lerp(icon.rotation, Quaternion.identity, Time.deltaTime * smoothSpeed);
                }
            }
            else // Target is behind the camera (screenPos.z <= 0)
            {
                if (icon != null) icon.gameObject.SetActive(false);
                if (distanceText != null) distanceText.gameObject.SetActive(false);
            }
        }

        // Always update distance text if active
        if (icon != null && icon.gameObject.activeSelf && distanceText != null)
        {
            float dist = Vector3.Distance(playerTransform.position, target.position);
            distanceText.text = Mathf.RoundToInt(dist) + " m";
        }
    }
}