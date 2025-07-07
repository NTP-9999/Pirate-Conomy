using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;
    public Button buyTabButton;
    public Button sellTabButton;
    public Transform itemListParent;
    public GameObject itemEntryPrefab;
    public GameObject itemDetailPanel;
    public TextMeshProUGUI itemNameText;
    public Image itemIconImage;
    public TextMeshProUGUI itemPriceText;
    public TMP_InputField amountInputField;
    public Button confirmButton;
    public TextMeshProUGUI totalPriceText;

    private ShopManager shopManager;
    private bool isBuyMode = true;
    private ShopItemData selectedItem;
    private int selectedAmount = 1;

    public void ShowShop(ShopManager manager)
    {
        shopManager = manager;
        shopPanel.SetActive(true);
        ShowBuyTab();
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
        itemDetailPanel.SetActive(false);
    }

    public void ShowBuyTab()
    {
        isBuyMode = true;
        buyTabButton.interactable = false;
        sellTabButton.interactable = true;
        PopulateItemList(shopManager.itemsForSale, true);
        itemDetailPanel.SetActive(false);
    }

    public void ShowSellTab()
    {
        isBuyMode = false;
        buyTabButton.interactable = true;
        sellTabButton.interactable = false;
        // TODO: ดึงไอเทมจาก InventoryManager ของผู้เล่น
        List<ShopItemData> playerItems = GetPlayerSellableItems();
        PopulateItemList(playerItems, false);
        itemDetailPanel.SetActive(false);
    }

    public void PopulateItemList(List<ShopItemData> items, bool isBuy)
    {
        // ลบของเก่าออกก่อน
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject entry = Instantiate(itemEntryPrefab, itemListParent);
            entry.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            entry.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
            entry.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = (isBuy ? item.buyPrice : item.sellPrice) + "";
            // CurrencyIcon สามารถเซ็ตเป็นรูปเหรียญ/เงินได้ตามต้องการ

            // เพิ่ม Event เวลาคลิกเลือกไอเทม
            Button btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnSelectItem(item));
            }
        }
    }

    public void OnSelectItem(ShopItemData item)
    {
        selectedItem = item;
        selectedAmount = 1;
        itemDetailPanel.SetActive(true);
        itemNameText.text = item.itemName;
        itemIconImage.sprite = item.icon;
        amountInputField.text = "1";
        UpdateDetailPanel();
    }

    public void OnAmountChanged(string value)
    {
        int.TryParse(value, out selectedAmount);
        selectedAmount = Mathf.Max(1, selectedAmount);
        UpdateDetailPanel();
    }

    private void UpdateDetailPanel()
    {
        if (selectedItem == null) return;
        int price = isBuyMode ? selectedItem.buyPrice : selectedItem.sellPrice;
        int total = price * selectedAmount;
        itemPriceText.text = price.ToString();
        totalPriceText.text = total.ToString();
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = isBuyMode ? "Buy" : "Sell";
    }

    public void OnConfirmButton()
    {
        if (selectedItem == null) return;
        if (isBuyMode)
        {
            // TODO: เชื่อมกับ PlayerCurrency และ InventoryManager จริง
            bool success = shopManager.BuyItem(selectedItem, selectedAmount, FindObjectOfType<PlayerCurrency>(), FindObjectOfType<InventoryManager>());
            if (success) HideShop();
        }
        else
        {
            bool success = shopManager.SellItem(selectedItem, selectedAmount, FindObjectOfType<PlayerCurrency>(), FindObjectOfType<InventoryManager>());
            if (success) HideShop();
        }
    }

    private List<ShopItemData> GetPlayerSellableItems()
    {
        // TODO: ดึงไอเทมจาก InventoryManager จริง ๆ
        // ตัวอย่าง: return FindObjectOfType<InventoryManager>().GetAllSellableItems();
        return new List<ShopItemData>();
    }
} 