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

        List<InventoryItem> playerResources = GetPlayerResourceItems();
        PopulateSellItemList(playerResources);
        itemDetailPanel.SetActive(false);
    }

    public void PopulateItemList(List<ShopItemData> items, bool isBuy)
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject entry = Instantiate(itemEntryPrefab, itemListParent);
            entry.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            entry.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
            entry.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = (isBuy ? item.buyPrice : item.sellPrice) + "";

            // ซ่อน Amount ในหน้า Buy
            var amountObj = entry.transform.Find("Amount");
            if (amountObj != null)
                amountObj.gameObject.SetActive(false);

            Button btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnSelectItem(item));
            }
        }
    }

    public void PopulateSellItemList(List<InventoryItem> items)
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject entry = Instantiate(itemEntryPrefab, itemListParent);
            entry.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            entry.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
            entry.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = GetSellPrice(item.itemName) + "";

            // โชว์ Amount ในหน้า Sell
            var amountObj = entry.transform.Find("Amount");
            if (amountObj != null)
            {
                amountObj.gameObject.SetActive(true);
                amountObj.GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();
            }

            Button btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnSelectSellItem(item));
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

        // ป้องกัน OnValueChanged trigger ตอนเซ็ตค่า
        amountInputField.onValueChanged.RemoveListener(OnAmountChanged);
        amountInputField.text = "1";
        amountInputField.onValueChanged.AddListener(OnAmountChanged);

        UpdateDetailPanel();
    }

    public void OnAmountChanged(string value)
    {
        if (selectedItem == null) return;
        int.TryParse(value, out selectedAmount);
        selectedAmount = Mathf.Max(1, selectedAmount);
        Debug.Log("OnAmountChanged: value=" + value + ", selectedAmount=" + selectedAmount);
        UpdateDetailPanel();
    }

    private void UpdateDetailPanel()
    {
        if (selectedItem == null) return;
        int price = isBuyMode ? selectedItem.buyPrice : selectedItem.sellPrice;
        int total = price * selectedAmount;
        Debug.Log($"selectedAmount={selectedAmount}, price={price}, total={total}");
        itemPriceText.text = price.ToString();
        totalPriceText.text = total.ToString();
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = isBuyMode ? "Buy" : "Sell";
    }

    public void OnConfirmButton()
    {
        if (selectedItem == null) return;
        if (isBuyMode)
        {
            bool success = shopManager.BuyItem(selectedItem, selectedAmount, FindObjectOfType<PlayerCurrency>(), FindObjectOfType<InventoryManager>());
        }
        else
        {
            bool success = shopManager.SellItem(selectedItem, selectedAmount, FindObjectOfType<PlayerCurrency>(), FindObjectOfType<InventoryManager>());
            if (success)
            {
                ShowSellTab();
                itemDetailPanel.SetActive(false);
            }
        }
    }

    private List<ShopItemData> GetPlayerSellableItems()
    {
        // TODO: ดึงไอเทมจาก InventoryManager จริง ๆ
        // ตัวอย่าง: return FindObjectOfType<InventoryManager>().GetAllSellableItems();
        return new List<ShopItemData>();
    }

    private List<InventoryItem> GetPlayerResourceItems()
    {
        var inventory = FindObjectOfType<InventoryManager>();
        List<InventoryItem> resources = new List<InventoryItem>();
        if (inventory == null) return resources;

        foreach (var item in inventory.items)
        {
            if (item.itemName == "Wood" || item.itemName == "Ore" || item.itemName == "Oil")
            {
                resources.Add(item);
            }
        }
        Debug.Log("Resource count: " + resources.Count);
        return resources;
    }

    private int GetSellPrice(string itemName)
    {
        var shopItem = shopManager.resourceSellableItems.Find(i => i.itemName == itemName);
        return shopItem != null ? shopItem.sellPrice : 0;
    }

    public void OnSelectSellItem(InventoryItem item)
    {
        selectedItem = shopManager.resourceSellableItems.Find(i => i.itemName == item.itemName);
        selectedAmount = 1;
        itemDetailPanel.SetActive(true);
        itemNameText.text = item.itemName;
        itemIconImage.sprite = item.icon;

        // ป้องกัน OnValueChanged trigger ตอนเซ็ตค่า
        amountInputField.onValueChanged.RemoveListener(OnAmountChanged);
        amountInputField.text = "1";
        amountInputField.onValueChanged.AddListener(OnAmountChanged);

        UpdateDetailPanel();
    }
} 