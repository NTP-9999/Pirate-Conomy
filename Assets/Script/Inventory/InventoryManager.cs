using UnityEngine;
using System.Collections.Generic;
public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your InventoryUI Panel (GameObject) here from the Hierarchy.")]
    [SerializeField] private GameObject inventoryUI; // อ้างอิงถึง GameObject ของ Inventory UI

    [Header("Key Bindings")]
    public KeyCode toggleInventoryKey = KeyCode.B; // ปุ่มสำหรับเปิด/ปิด Inventory

    private bool isInventoryOpen = false; // สถานะปัจจุบันของ Inventory (เปิด/ปิด)
    public static InventoryManager Instance;
    private InventoryNotificationUI notificationUI;
    [Header("Slot References")]
    public InventorySlotUI[] slots; 

    public List<InventoryItem> items = new List<InventoryItem>();
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public Sprite icon;
    }

    public List<ItemData> itemDatabase;
    public Sprite GetItemIcon(string itemName)
    {
        return itemDatabase.Find(i => i.itemName == itemName)?.icon;
    }

    void Awake()
    {
        notificationUI = FindObjectOfType<InventoryNotificationUI>();
        if (notificationUI == null)
        {
            Debug.LogWarning("InventoryNotificationUI not found in scene.");
        }
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
    /// 
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // สลับค่า true/false
        inventoryUI.SetActive(isInventoryOpen); // เปิดหรือปิด GameObject ของ UI

        // ควบคุมการหยุดเวลาหรือเคอร์เซอร์ของเมาส์
        if (isInventoryOpen)
        {
            // Inventory เปิด: หยุดเวลา, แสดงเคอร์เซอร์เมาส์
            Time.timeScale = 0f; // หยุดเวลาในเกม (Freeze game)
            Cursor.lockState = CursorLockMode.None; // ปลดล็อกเมาส์
            Cursor.visible = true; // ทำให้เมาส์มองเห็นได้
            Debug.Log("Inventory Opened!");
        }
        else
        {
            // Inventory ปิด: กลับมาเล่นเกม, ซ่อนเคอร์เซอร์เมาส์
            Time.timeScale = 1f; // กลับมาเล่นเกมตามปกติ
            Cursor.lockState = CursorLockMode.Locked; // ล็อกเมาส์ไว้ที่กึ่งกลางจอ
            Cursor.visible = false; // ซ่อนเมาส์
            Debug.Log("Inventory Closed!");
        }
    }
    public void AddItem(string itemName, Sprite icon, int amount)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.Add(amount);
        }
        else
        {
            item = new InventoryItem(itemName, icon, amount);
            items.Add(item);
        }
        UpdateSlots();
        ShowPickupToast(item, item.quantity);
    }
    void UpdateSlots()
    {
        // ล้างทุกช่องก่อน
        foreach (var slot in slots)
            slot.ClearSlot();

        // แสดงไอเทมในช่องต่างๆ
        for (int i = 0; i < items.Count && i < slots.Length; i++)
        {
            InventoryItem item = items[i]; // เพิ่มบรรทัดนี้
            slots[i].SetItem(item.itemName, item.icon, item.quantity); // แก้ไขบรรทัดนี้
        }
    }

    void ShowPickupToast(InventoryItem item, int totalAmount)
    {
        notificationUI.ShowNotification(item, totalAmount);
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
    public void RemoveItem(string itemName, int amount)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.quantity -= amount;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
            UpdateSlots();
        }
    }
    public void UseItem(string itemName)
    {
        var item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            // ตรงนี้เป็น logic consume: สมมติว่าเราจะ hardcode ไปก่อนว่า item อะไรทำอะไร
            if (itemName == "Fried Chicken") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(20);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Grilled MHeal") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(10);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Grilled Squid") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(5);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Hard Bread") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(30);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Salted Fish") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(15);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Sashimi Salmon") // เป็นอาหาร
            {
                CharacterStats.Instance.Heal(40);
                Debug.Log("Used food item: " + itemName);
            }
            else if (itemName == "Rum") // ลด stress
            {
                CharacterStats.Instance.DecreaseStress(40);
                Debug.Log("Used stress relief item: " + itemName);
            }
            else if (itemName == "Water")
            {
                CharacterStats.Instance.DecreaseStress(20);
                Debug.Log("Used stress relief item: " + itemName);
            }
            else
            {
                Debug.Log(itemName + " is a quest item or cannot be used.");
                return; // ไม่ต้อง remove
            }

            RemoveItem(itemName, 1); // ใช้แล้วลดจำนวน
        }
    }
}