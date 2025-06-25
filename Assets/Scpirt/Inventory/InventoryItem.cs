using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public int quantity;

    public InventoryItem(string name, Sprite icon, int amount = 1)
    {
        this.itemName = name;
        this.icon = icon;
        this.quantity = amount;
    }

    public void Add(int amount)
    {
        quantity += amount;
    }
}
