using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI countText;

    private string itemName = "";
    private int quantity = 0;

    // เช็คว่าเป็นช่องว่าง
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(itemName);
    }

    public string GetItemName()
    {
        return itemName;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
        UpdateUI();
    }

    public void SetItem(string name, Sprite icon, int amount)
    {
        itemName = name;
        quantity = amount;

        iconImage.sprite = icon;
        iconImage.enabled = true;

        UpdateUI();
    }

    public void ClearSlot()
    {
        itemName = "";
        quantity = 0;

        iconImage.sprite = null;
        iconImage.enabled = false;

        countText.text = "";
        countText.enabled = false;
    }

    private void UpdateUI()
    {
        countText.text = $"x{quantity}";
        countText.enabled = true;
    }
    public void OnSlotClicked()
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            InventoryManager.Instance.UseItem(itemName);
        }
    }
}
