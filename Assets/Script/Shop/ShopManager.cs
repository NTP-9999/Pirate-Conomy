using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items (ซื้อได้)")]
    public List<ShopItemData> itemsForSale; // สำหรับไอเทมที่ซื้อได้

    [Header("Shop Items (ขายได้)")]
    public List<ShopItemData> resourceSellableItems; // สำหรับ Wood, Ore, Oil

    [Header("UI Reference")]
    public ShopUI shopUI;

    [System.Serializable]
    public class CustomSellPrice
    {
        public ShopItemData item;
        public int sellPrice;
    }
    public List<CustomSellPrice> customSellPrices;

    public bool BuyItem(ShopItemData item, int amount, PlayerCurrency playerCurrency, InventoryManager inventory)
    {
        int totalPrice = item.buyPrice * amount;
        if (playerCurrency.currentNova >= totalPrice)
        {
            if (playerCurrency.SpendNova(totalPrice))
            {
                inventory.AddItem(item.itemName, item.icon, amount);
                return true;
            }
        }
        return false;
    }

    public bool SellItem(ShopItemData item, int amount, PlayerCurrency playerCurrency, InventoryManager inventory)
    {
        InventoryItem invItem = inventory.items.Find(i => i.itemName == item.itemName);
        if (invItem != null && invItem.quantity >= amount)
        {
            inventory.RemoveItem(item.itemName, amount);
            // ดึงราคาขายจาก customSellPrices ถ้ามี
            int sellPrice = item.sellPrice;
            var custom = customSellPrices != null ? customSellPrices.Find(c => c.item == item) : null;
            if (custom != null)
                sellPrice = custom.sellPrice;
            int totalSell = sellPrice * amount;
            playerCurrency.AddNova(totalSell);
            return true;
        }
        return false;
    }

    // เรียกตอนเปิดร้านค้า
    public void OpenShop()
    {
        shopUI.ShowShop(this);
    }
    
} 