using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointUI : MonoBehaviour
{
    [Header("UI References (Assign in Prefab Inspector)")]
    [Tooltip("Drag the Waypoint Icon Image (e.g., arrow sprite) here from this prefab's children.")]
    [SerializeField] private Image waypointIcon; // ไอคอนลูกศร
    [Tooltip("Drag the Distance Text (TextMeshProUGUI) here from this prefab's children.")]
    [SerializeField] private TextMeshProUGUI distanceText; // ข้อความระยะทาง
    
    // ไม่ต้องมี [SerializeField] ตรงนี้แล้ว เพราะเราจะหาใน Awake()
    private Transform playerTransform; 
    private Camera mainCamera; 

    [Header("Settings")]
    [Tooltip("Offset from the screen edges where the waypoint will stop when off-screen.")]
    public float borderOffset = 80f; // ปรับลดค่า Default ลงมาหน่อย
    [Tooltip("Distance at which the waypoint UI will disappear (e.g., when player is very close to objective).")]
    public float minDistanceToHide = 5f; 

    private RectTransform rectTransform; 
    private Vector3 screenCenter; 
    private float screenWidth;
    private float screenHeight;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // *** การหา PlayerTransform ในรันไทม์ ***
        // จะพยายามหา GameObject ที่มี Tag "Player"
        if (playerTransform == null) // ตรวจสอบว่ายังไม่ได้กำหนดค่า
        {
            GameObject playerGO = GameObject.FindWithTag("Player"); 
            if (playerGO != null)
            {
                playerTransform = playerGO.transform;
            }
            else
            {
                // Fallback: ถ้าหา Player ไม่เจอ, ลองใช้ Main Camera เป็นตำแหน่งอ้างอิงแทน
                Debug.LogWarning("WaypointUI: Player Transform not found with 'Player' tag. Falling back to Main Camera. Ensure your player has the 'Player' tag for accurate positioning.", this);
                if (Camera.main != null) // ตรวจสอบว่า Main Camera มีอยู่
                {
                    playerTransform = Camera.main.transform;
                }
                else
                {
                    Debug.LogError("WaypointUI: Failed to find Player Transform (tag 'Player') and Main Camera. Waypoint UI will not function. Make sure at least one exists and is properly tagged.", this);
                    enabled = false; // ปิดสคริปต์นี้ถ้าหา Reference สำคัญไม่เจอ
                    return;
                }
            }
        }

        // *** การหา Main Camera ในรันไทม์ ***
        // Camera.main จะหา GameObject ที่มี Tag "MainCamera" โดยอัตโนมัติ
        if (mainCamera == null) // ตรวจสอบว่ายังไม่ได้กำหนดค่า
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("WaypointUI: Main Camera not found! Ensure your main camera GameObject has the 'MainCamera' tag.", this);
                enabled = false; // ปิดสคริปต์นี้ถ้าหา Reference สำคัญไม่เจอ
                return;
            }
        }

        // *** ตรวจสอบ UI References ที่ต้องลากใส่ใน Prefab Inspector ***
        if (waypointIcon == null)
        {
            Debug.LogError("WaypointUI: Waypoint Icon Image is NOT ASSIGNED in the _WaypointUIPrefab! Please assign it in the prefab inspector.", this);
            enabled = false;
            return;
        }
        if (distanceText == null)
        {
            Debug.LogError("WaypointUI: Distance Text is NOT ASSIGNED in the _WaypointUIPrefab! Please assign it in the prefab inspector.", this);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // คำนวณขนาดหน้าจอเมื่อเริ่มต้น (เผื่อมีการเปลี่ยน Resolution)
        UpdateScreenDimensions();
    }

    // OnRectTransformDimensionsChange จะถูกเรียกเมื่อขนาดของ RectTransform หรือ Parent เปลี่ยนแปลง
    void OnRectTransformDimensionsChange()
    {
        UpdateScreenDimensions();
    }

    private void UpdateScreenDimensions()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenCenter = new Vector3(screenWidth / 2, screenHeight / 2, 0);
    }

    /// <summary>
    /// อัปเดตตำแหน่งและข้อมูลของ Waypoint UI (ถูกเรียกโดย ObjectiveManager)
    /// </summary>
    /// <param name="target">Transform ของเป้าหมาย</param>
    public void UpdateWaypoint(Transform target)
    {
        // ตรวจสอบ null อีกครั้งเพื่อความปลอดภัย (เผื่อบางสิ่งถูกทำลายไปแล้ว)
        if (target == null || playerTransform == null || mainCamera == null)
        {
            // ถ้า Reference ไม่ครบ ให้ซ่อน UI และออก
            gameObject.SetActive(false); 
            return;
        }

        // คำนวณระยะทาง
        float distance = Vector3.Distance(playerTransform.position, target.position);
        distanceText.text = $"{distance:0}m"; // แสดงระยะทางเป็นจำนวนเต็ม เมตร

        // ซ่อน Waypoint ถ้าผู้เล่นอยู่ใกล้เป้าหมายมากพอ
        if (distance <= minDistanceToHide)
        {
            waypointIcon.enabled = false;
            distanceText.enabled = false;
            return; // ไม่ต้องประมวลผลต่อ
        }
        else
        {
            waypointIcon.enabled = true;
            distanceText.enabled = true;
        }

        // แปลงตำแหน่ง 3D ของเป้าหมายเป็นตำแหน่ง 2D บนหน้าจอ
        Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);

        // *** การจัดการเป้าหมายที่อยู่ด้านหลังกล้อง ***
        // ถ้า targetScreenPosition.z < 0 หมายถึงเป้าหมายอยู่ด้านหลังกล้อง
        if (targetScreenPosition.z < 0)
        {
            // Mirror ตำแหน่ง เพื่อให้ Waypoint ชี้ไปทางที่ถูกต้องบนขอบจอตรงกันข้าม
            targetScreenPosition.x = screenWidth - targetScreenPosition.x;
            targetScreenPosition.y = screenHeight - targetScreenPosition.y;
            targetScreenPosition.z = 0; // ตั้ง Z เป็น 0 เพื่อให้ไม่เกิดปัญหาการ render
        }

        // *** Clamp ตำแหน่ง UI ให้อยู่ในขอบเขตหน้าจอพร้อม Offset ***
        float clampedX = Mathf.Clamp(targetScreenPosition.x, borderOffset, screenWidth - borderOffset);
        float clampedY = Mathf.Clamp(targetScreenPosition.y, borderOffset, screenHeight - borderOffset);
        
        rectTransform.position = new Vector3(clampedX, clampedY, 0);

        // *** Logic สำหรับการหมุนไอคอนเมื่อเป้าหมายอยู่นอกจอจริง ๆ ***
        // การตรวจสอบนี้แยกกับการ Clamp ตำแหน่ง เพื่อให้รู้ว่าควรหมุนไอคอนหรือไม่
        bool isOffScreenForRotation = 
            targetScreenPosition.x < borderOffset || targetScreenPosition.x > screenWidth - borderOffset ||
            targetScreenPosition.y < borderOffset || targetScreenPosition.y > screenHeight - borderOffset ||
            targetScreenPosition.z < 0; // ยังถือว่า Off-screen หากเดิมอยู่หลังกล้อง

        if (isOffScreenForRotation)
        {
            // คำนวณทิศทางจากจุดกึ่งกลางหน้าจอไปยังตำแหน่งเป้าหมายที่ถูก Clamp
            // (หรือจากตำแหน่งจริงของเป้าหมายก่อน Clamp ถ้าต้องการให้ชี้ทิศทางแม่นยำกว่า)
            // ณ จุดนี้ targetScreenPosition คือตำแหน่งที่ถูก Mirror ไปแล้วถ้าอยู่หลังกล้อง
            Vector3 direction = targetScreenPosition - screenCenter;
            direction.z = 0; // ตัดแกน Z ออกไป

            // ตรวจสอบว่า direction ไม่ใช่ Vector ศูนย์ เพื่อป้องกันการคำนวณที่ผิดพลาด
            if (direction.sqrMagnitude > 0.001f) // sqrMagnitude ดีกว่า magnitude เพื่อประสิทธิภาพ
            {
                direction.Normalize(); // ทำให้เป็น Vector ขนาด 1

                // คำนวณมุมหมุนของไอคอนให้ชี้ไปทางเป้าหมาย
                // Mathf.Atan2(y, x) ให้มุมเป็นเรเดียน, * Mathf.Rad2Deg เพื่อแปลงเป็นองศา
                // -90 เพราะ Sprite อาจจะหันขึ้น (ต้องหมุน 90 องศาเพื่อให้ชี้ขวา)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rectTransform.localEulerAngles = new Vector3(0, 0, angle - 90); 
            }
            else // ถ้า direction เป็น Vector ศูนย์ (เช่น target อยู่ตรงกลางหน้าจอเป๊ะ)
            {
                rectTransform.localEulerAngles = Vector3.zero; // ไม่ต้องหมุน
            }
        }
        else // เป้าหมายอยู่ในจอและอยู่ด้านหน้ากล้อง
        {
            rectTransform.localEulerAngles = Vector3.zero; // ไม่ต้องหมุนไอคอน
        }
    }
}