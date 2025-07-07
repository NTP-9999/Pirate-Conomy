using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items (ขาย)")]
    public List<ShopItemData> itemsForSale;

    [Header("UI Reference")]
    public ShopUI shopUI;

    // ตัวอย่างฟังก์ชันซื้อไอเทม
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

    // ตัวอย่างฟังก์ชันขายไอเทม
    public bool SellItem(ShopItemData item, int amount, PlayerCurrency playerCurrency, InventoryManager inventory)
    {
        // เช็คว่ามีไอเทมพอไหม
        InventoryItem invItem = inventory.items.Find(i => i.itemName == item.itemName);
        if (invItem != null && invItem.quantity >= amount)
        {
            inventory.RemoveItem(item.itemName, amount); // ต้องสร้างฟังก์ชันนี้
            int totalSell = item.sellPrice * amount;
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