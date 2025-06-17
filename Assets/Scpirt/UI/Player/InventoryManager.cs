using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your InventoryUI Panel (GameObject) here from the Hierarchy.")]
    [SerializeField] private GameObject inventoryUI; // อ้างอิงถึง GameObject ของ Inventory UI

    [Header("Key Bindings")]
    public KeyCode toggleInventoryKey = KeyCode.B; // ปุ่มสำหรับเปิด/ปิด Inventory

    private bool isInventoryOpen = false; // สถานะปัจจุบันของ Inventory (เปิด/ปิด)

    void Awake()
    {
        // ตรวจสอบว่าได้ลาก InventoryUI มาใส่ใน Inspector หรือไม่
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryManager: InventoryUI GameObject is not assigned! Please assign it in the Inspector.", this);
            enabled = false; // ปิด Script ถ้าไม่มี UI Reference
            return;
        }

        // ตรวจสอบให้แน่ใจว่า InventoryUI ถูกปิดไว้ในตอนเริ่มต้น
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        // ตรวจจับการกดปุ่ม B
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            ToggleInventory(); // เรียกเมธอดสำหรับสลับสถานะ Inventory
        }
    }

    /// <summary>
    /// สลับสถานะการเปิด/ปิดหน้าต่าง Inventory
    /// </summary>
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // สลับค่า true/false
        inventoryUI.SetActive(isInventoryOpen); // เปิดหรือปิด GameObject ของ UI

        // ควบคุมการหยุดเวลาหรือเคอร์เซอร์ของเมาส์
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // ปลดล็อกเมาส์
            Cursor.visible = true; // ทำให้เมาส์มองเห็นได้
            Debug.Log("Inventory Opened!");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // ล็อกเมาส์ไว้ที่กึ่งกลางจอ
            Cursor.visible = false; // ซ่อนเมาส์
            Debug.Log("Inventory Closed!");
        }
    }

    /// <summary>
    /// เมธอดสำหรับบังคับปิด Inventory (อาจใช้ในกรณีที่ผู้เล่นตาย หรือ Event อื่นๆ)
    /// </summary>
    public void CloseInventory()
    {
        if (isInventoryOpen)
        {
            isInventoryOpen = false;
            inventoryUI.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Inventory forcefully closed.");
        }
    }

    /// <summary>
    /// Property สำหรับตรวจสอบสถานะ Inventory จากสคริปต์อื่น
    /// </summary>
    public bool IsInventoryOpen
    {
        get { return isInventoryOpen; }
    }
}