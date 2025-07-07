using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Shop/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int buyPrice;
    public int sellPrice;
    [TextArea]
    public string description;
    // สามารถเพิ่ม field อื่น ๆ ได้ตามต้องการ เช่น ประเภทไอเทม ฯลฯ
} 