using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public InventorySlotUI[] slots; // ลาก Slot_0~Slot_9 ใส่นี่

    public void AddItem(string itemName, Sprite icon, int amount)
    {
        // ถ้ามีไอเทมนี้แล้ว → บวกจำนวน
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.GetItemName() == itemName)
            {
                slot.AddQuantity(amount);
                return;
            }
        }

        // ถ้ายังไม่มี → หาช่องว่าง
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(itemName, icon, amount);
                return;
            }
        }

        Debug.LogWarning("Inventory เต็มแล้ว ไม่สามารถเพิ่ม " + itemName);
    }
}
