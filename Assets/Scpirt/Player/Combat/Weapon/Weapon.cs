using UnityEngine;

// Enum สำหรับระบุประเภทของอาวุธ
public enum WeaponType
{
    Melee,  // สำหรับดาบ หรืออาวุธระยะประชิด
    Ranged  // สำหรับปืน หรืออาวุธระยะไกล
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Base Settings")]
    [SerializeField] protected string _weaponName = "New Weapon";
    [SerializeField] protected WeaponType _weaponType = WeaponType.Melee;

    // Properties สำหรับเข้าถึงค่าต่างๆ ของอาวุธ
    public string WeaponName => _weaponName;
    public WeaponType Type => _weaponType;

    // เพิ่ม Reference สำหรับ Animator และ BasicCombat
    protected Animator playerAnimator; // Animator ของผู้เล่น (จะถูกกำหนดจาก BasicCombat)
    protected BasicCombat basicCombat; // BasicCombat ของผู้เล่น (จะถูกกำหนด)

    // เมธอด Start ที่สามารถ Override ได้ในคลาสลูก
    protected virtual void Start()
    {
        // ตัวอย่างการหา BasicCombat จาก ComponentInParent
        basicCombat = GetComponentInParent<BasicCombat>();
        if (basicCombat != null)
        {
            playerAnimator = basicCombat.playerAnimator; // **แก้ไขตรงนี้: เปลี่ยนเป็น basicCombat.playerAnimator**
        }
        else
        {
            Debug.LogWarning($"Weapon: BasicCombat not found in parent hierarchy for {gameObject.name}. Attack animations might not be handled.", this);
            // ถ้าหา BasicCombat ไม่เจอ อาจจะลองหา Animator บน GameObject ของ Player โดยตรง
            playerAnimator = GetComponentInParent<Animator>(); // สมมติว่า Animator อยู่บน GameObject แม่
            if (playerAnimator == null)
            {
                Debug.LogWarning($"Weapon: Player Animator not found in parent hierarchy for {gameObject.name}. Cannot play animations.", this);
            }
        }
    }

    // เมธอดนี้จะถูกเรียกเมื่ออาวุธถูกหยิบขึ้นมาถือ
    public virtual void Equip()
    {
        gameObject.SetActive(true);
        Debug.Log($"Equipped: {WeaponName} ({Type})");
    }

    // เมธอดนี้จะถูกเรียกเมื่ออาวุธถูกเก็บหรือสลับไปถืออาวุธอื่น
    public virtual void Unequip()
    {
        gameObject.SetActive(false);
        Debug.Log($"Unequipped: {WeaponName} ({Type})");
    }

    // เมธอด Abstract สำหรับการโจมตี ที่คลาสลูกต้อง implements
    // นี่คือเมธอดที่ PlayerCombatController จะเรียกผ่าน PlayerWeaponManager
    public abstract void Attack();
}