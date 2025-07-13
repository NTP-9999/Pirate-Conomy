using UnityEngine;

public enum ConsumableType
{
    Food,
    StressRelief,
    Quest
}

[CreateAssetMenu(fileName = "NewConsumableItem", menuName = "Items/ConsumableItem")]
public class ConsumableItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ConsumableType type;
    public float value; // ถ้าเป็นอาหาร = +Hunger, ถ้า stress relief = -Stress, ถ้า Quest = 0
}
