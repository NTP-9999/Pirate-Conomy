using UnityEngine;
using TMPro; // จำเป็นสำหรับ TextMeshProUGUI

public class ObjectiveManager : MonoBehaviour
{
    // *** Singleton Pattern ***
    public static ObjectiveManager Instance { get; private set; } 

    [Header("Current Objective")]
    [Tooltip("The GameObject representing the current objective's physical location.")]
    public Transform currentObjectiveTarget; // ตำแหน่งเป้าหมายในโลก
    [Tooltip("The text description of the current objective (e.g., 'Defeat all enemies').")]
    public string currentObjectiveText = "No current objective."; // ข้อความเป้าหมาย

    [Header("UI References")]
    [Tooltip("Drag the Waypoint UI Prefab (with WaypointUI.cs) here.")]
    [SerializeField] private GameObject waypointUIPrefab; // Prefab ของ Waypoint UI
    [Tooltip("The RectTransform of the Canvas or a parent UI element for waypoints.")]
    [SerializeField] private RectTransform waypointParentCanvas; // Canvas ที่ Waypoint UI จะเป็น Child
    [Tooltip("Drag your TextMeshProUGUI element for objective text here.")]
    [SerializeField] private TextMeshProUGUI objectiveTextUI; // Text UI สำหรับข้อความเป้าหมายที่มุมขวาบน

    // ตัวแปรสำหรับ Waypoint UI ที่ถูกสร้างขึ้นมา (จัดการโดย ObjectiveManager)
    private WaypointUI activeWaypointUI; // Reference ไปยัง WaypointUI Component ที่ Instantiate ขึ้นมา

    void Awake()
    {
        // *** Singleton Setup Logic ***
        if (Instance != null && Instance != this)
        {
            // ถ้ามี Instance อื่นอยู่แล้ว และไม่ใช่ตัวเรา ให้ทำลายตัวนี้ทิ้ง
            Debug.LogWarning($"ObjectiveManager: Duplicate instance of {typeof(ObjectiveManager).Name} found. Destroying this one.", this);
            Destroy(gameObject);
        }
        else
        {
            // ถ้ายังไม่มี Instance หรือตัวเราคือ Instance แรก ให้กำหนดให้ตัวนี้เป็น Instance
            Instance = this;
            // ทำให้ ObjectiveManager ไม่ถูกทำลายเมื่อเปลี่ยน Scene เพื่อให้ข้อมูลเป้าหมายคงอยู่
            DontDestroyOnLoad(gameObject); 
            Debug.Log($"ObjectiveManager: Initialized as Singleton and set to DontDestroyOnLoad.");
        }

        // ตรวจสอบ Reference UI ที่จำเป็น (ไม่ควรเป็น null)
        // ถ้ามีอันไหนเป็น null สคริปต์จะไม่ทำงานและแจ้งเตือน
        if (waypointUIPrefab == null)
        {
            Debug.LogError("ObjectiveManager: Waypoint UI Prefab is NOT ASSIGNED! Please assign it in the Inspector.", this);
            enabled = false; // ปิด Script ถ้าไม่มี UI Reference
            return;
        }
        if (waypointParentCanvas == null)
        {
            Debug.LogError("ObjectiveManager: Waypoint Parent Canvas is NOT ASSIGNED! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }
        if (objectiveTextUI == null)
        {
            Debug.LogError("ObjectiveManager: Objective Text UI (TextMeshProUGUI) is NOT ASSIGNED! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }

        // สร้าง Waypoint UI ขึ้นมา 1 ตัวใน Awake และเก็บ Reference ไว้
        GameObject go = Instantiate(waypointUIPrefab, waypointParentCanvas);
        activeWaypointUI = go.GetComponent<WaypointUI>();

        if (activeWaypointUI == null)
        {
            Debug.LogError("ObjectiveManager: WaypointUI prefab does NOT have WaypointUI script attached! Please ensure it's on the prefab.", this);
            enabled = false;
            return;
        }
        activeWaypointUI.gameObject.SetActive(false); // ซ่อน Waypoint UI ไว้ก่อนในตอนเริ่มต้น

        // อัปเดต UI ครั้งแรก (จะแสดงข้อความ "No current objective." และซ่อน Waypoint)
        UpdateObjectiveUI(); 
    }

    void Update()
    {
        // อัปเดตข้อความเป้าหมายใน UI ทุกเฟรม
        objectiveTextUI.text = currentObjectiveText;

        // ถ้ามีเป้าหมาย:
        if (currentObjectiveTarget != null)
        {
            // ถ้า Waypoint UI ยังไม่ Active ให้เปิดใช้งาน
            if (!activeWaypointUI.gameObject.activeSelf)
            {
                activeWaypointUI.gameObject.SetActive(true);
            }
            // สั่งให้ Waypoint UI อัปเดตตัวเองตามตำแหน่งเป้าหมาย
            activeWaypointUI.UpdateWaypoint(currentObjectiveTarget);
        }
        // ถ้าไม่มีเป้าหมาย:
        else
        {
            // ถ้า Waypoint UI ยัง Active ให้ปิดใช้งาน
            if (activeWaypointUI.gameObject.activeSelf)
            {
                activeWaypointUI.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// กำหนดเป้าหมายใหม่: ใช้เมธอดนี้จากสคริปต์ภารกิจหรือ Event อื่นๆ
    /// </summary>
    /// <param name="targetTransform">Transform ของ GameObject ที่เป็นเป้าหมาย</param>
    /// <param name="objectiveDescription">ข้อความที่บอกว่าต้องทำอะไร</param>
    public void SetNewObjective(Transform targetTransform, string objectiveDescription)
    {
        currentObjectiveTarget = targetTransform;
        currentObjectiveText = objectiveDescription;
        UpdateObjectiveUI(); // อัปเดตการแสดงผลทันที
        Debug.Log($"ObjectiveManager: New Objective set: '{objectiveDescription}' at {targetTransform.name}");
    }

    /// <summary>
    /// ลบเป้าหมายปัจจุบัน: ใช้เมธอดนี้เมื่อภารกิจเสร็จสิ้น
    /// </summary>
    public void ClearObjective()
    {
        currentObjectiveTarget = null; // ตั้งเป้าหมายเป็น null
        currentObjectiveText = "No current objective."; // ตั้งข้อความเป็นค่าเริ่มต้น
        UpdateObjectiveUI(); // อัปเดตการแสดงผลทันที
        Debug.Log("ObjectiveManager: Current Objective Cleared.");
    }

    /// <summary>
    /// อัปเดตการแสดงผลของ UI ที่เกี่ยวข้องกับ Objective (ใช้ภายในคลาส)
    /// </summary>
    private void UpdateObjectiveUI()
    {
        objectiveTextUI.text = currentObjectiveText; // อัปเดตข้อความบนจอ

        // ซ่อนหรือแสดง Waypoint UI ตามสถานะของ currentObjectiveTarget
        if (activeWaypointUI != null)
        {
            activeWaypointUI.gameObject.SetActive(currentObjectiveTarget != null);
        }
    }
}