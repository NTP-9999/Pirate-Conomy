using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplay : MonoBehaviour
{
    public Image iconImage;
    public Image background;
    public TextMeshProUGUI itemNameText;

    public void SetUp(InventoryItem item, int amount)
    {
        iconImage.sprite = item.icon;
        itemNameText.text = $"{item.itemName} x{amount}";
    }
}
