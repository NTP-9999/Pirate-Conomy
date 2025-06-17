using UnityEngine;

// Enum สำหรับระบุประเภทของอาวุธ
public enum WeaponType
{
    Melee,  // สำหรับดาบ หรืออาวุธระยะประชิด
    Ranged  // สำหรับปืน หรืออาวุธระยะไกล
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Base Settings")]
    [SerializeField] private string _weaponName = "New Weapon";
    [SerializeField] private WeaponType _weaponType = WeaponType.Melee;

    // Properties สำหรับเข้าถึงค่าต่างๆ ของอาวุธ
    public string WeaponName => _weaponName;
    public WeaponType Type => _weaponType;

    // เมธอดนี้จะถูกเรียกเมื่ออาวุธถูกหยิบขึ้นมาถือ
    public virtual void Equip()
    {
        // เปิดใช้งาน GameObject ของอาวุธ
        gameObject.SetActive(true);
        Debug.Log($"Equipped: {WeaponName} ({Type})");
    }

    // เมธอดนี้จะถูกเรียกเมื่ออาวุธถูกเก็บหรือสลับไปถืออาวุธอื่น
    public virtual void Unequip()
    {
        // ปิดใช้งาน GameObject ของอาวุธ
        gameObject.SetActive(false);
        Debug.Log($"Unequipped: {WeaponName} ({Type})");
    }
}