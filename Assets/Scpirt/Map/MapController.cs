using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mapUI; // ตัว fullscreen map (เปิด/ปิด)
    public RawImage mapBackground; // รูปแผนที่
    public RectTransform playerMarker; // ตัวชี้ผู้เล่น

    [Header("World Map Settings")]
    public Vector2 worldMin; // พิกัดซ้ายล่างของโลกจริง (x, z)
    public Vector2 worldMax; // พิกัดขวาบนของโลกจริง (x, z)

    [Header("Map Movement Settings")]
    public RectTransform mapContainer; // ที่ใส่ mapBackground และขยับ
    public float mapScale = 1f;

    private Transform player;

    void Start()
    {
        var bounds = FindObjectOfType<WorldBoundsGizmo>();
        if (bounds != null)
        {
            worldMin = bounds.worldMin;
            worldMax = bounds.worldMax;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mapUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapUI.SetActive(!mapUI.activeSelf);
        }

        if (mapUI.activeSelf && player != null)
        {
            UpdateMapPosition();
        }
    }

    void UpdateMapPosition()
    {
        Vector2 normalizedPos = new Vector2(
            Mathf.InverseLerp(worldMin.x, worldMax.x, player.position.x),
            Mathf.InverseLerp(worldMin.y, worldMax.y, player.position.z)
        );

        // ขยับภาพแผนที่สวนทาง เพื่อให้ PlayerMarker อยู่กลางตลอด
        Vector2 mapSize = mapBackground.rectTransform.sizeDelta * mapScale;
        Vector2 offset = new Vector2(
            -normalizedPos.x * mapSize.x + (mapContainer.rect.width / 2),
            -normalizedPos.y * mapSize.y + (mapContainer.rect.height / 2)
        );

        mapBackground.rectTransform.anchoredPosition = offset;
    }
}
