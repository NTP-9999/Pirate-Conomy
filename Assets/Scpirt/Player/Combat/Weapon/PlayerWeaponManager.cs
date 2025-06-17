using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon References")]
    [Tooltip("Drag the GameObject of your Melee Weapon here (the one with MeleeWeapon.cs attached).")]
    public MeleeWeapon meleeWeapon;
    [Tooltip("Drag the GameObject of your Ranged Weapon here (the one with RangedWeapon.cs attached).")]
    public RangedWeapon rangedWeapon;

    [Header("Current Weapon State")]
    [SerializeField]
    private WeaponType _currentWeaponType = WeaponType.Melee; // กำหนดค่าเริ่มต้นเป็นดาบ
    public Weapon CurrentWeapon { get; private set; } // Property สำหรับเข้าถึงอาวุธที่ถืออยู่ปัจจุบัน

    void Start()
    {
        // ตรวจสอบว่ามี Reference อาวุธครบถ้วนหรือไม่
        if (meleeWeapon == null)
        {
            Debug.LogError("PlayerWeaponManager: MeleeWeapon reference is missing! Please assign it in the Inspector.", this);
            enabled = false; // ปิด Script ถ้าไม่มีอาวุธ (ป้องกัน Null Reference Errors)
            return;
        }
        if (rangedWeapon == null)
        {
            Debug.LogError("PlayerWeaponManager: RangedWeapon reference is missing! Please assign it in the Inspector.", this);
            enabled = false; // ปิด Script ถ้าไม่มีอาวุธ
            return;
        }

        // เริ่มต้นให้ผู้เล่นถืออาวุธตามที่ _currentWeaponType กำหนด (ซึ่งตอนนี้คือ Melee/ดาบ)
        EquipWeapon(_currentWeaponType);
    }

    void Update()
    {
        // ตรวจจับ Input สำหรับสลับอาวุธ
        if (Input.GetKeyDown(KeyCode.Alpha1)) // ถ้าผู้เล่นกดปุ่ม '1' (เหนือตัว QWERTY)
        {
            // ตรวจสอบว่าไม่ได้ถือดาบอยู่แล้ว เพื่อไม่ให้เรียก EquipWeapon ซ้ำซ้อน
            if (_currentWeaponType != WeaponType.Melee)
            {
                EquipWeapon(WeaponType.Melee); // สลับไปถือดาบ
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // ถ้าผู้เล่นกดปุ่ม '2'
        {
            // ตรวจสอบว่าไม่ได้ถือปืนอยู่แล้ว
            if (_currentWeaponType != WeaponType.Ranged)
            {
                EquipWeapon(WeaponType.Ranged); // สลับไปถือปืน
            }
        }
    }

    /// <summary>
    /// สลับไปถืออาวุธตามประเภทที่กำหนด และจัดการการเปิด/ปิด GameObject ของอาวุธ
    /// </summary>
    /// <param name="typeToEquip">ประเภทอาวุธที่ต้องการจะถือ (WeaponType.Melee หรือ WeaponType.Ranged)</param>
    public void EquipWeapon(WeaponType typeToEquip)
    {
        // 1. ถ้ามีอาวุธที่ถืออยู่ปัจจุบัน ให้สั่ง Unequip ก่อน
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Unequip();
        }

        // 2. กำหนดอาวุธใหม่ตามประเภทที่ต้องการ
        switch (typeToEquip)
        {
            case WeaponType.Melee:
                CurrentWeapon = meleeWeapon; // กำหนดอาวุธที่ถืออยู่เป็นดาบ
                _currentWeaponType = WeaponType.Melee; // อัปเดตประเภทอาวุธปัจจุบัน
                break;
            case WeaponType.Ranged:
                CurrentWeapon = rangedWeapon; // กำหนดอาวุธที่ถืออยู่เป็นปืน
                _currentWeaponType = WeaponType.Ranged; // อัปเดตประเภทอาวุธปัจจุบัน
                break;
            default:
                Debug.LogWarning($"PlayerWeaponManager: Attempted to equip unknown weapon type: {typeToEquip}");
                CurrentWeapon = null; // ถ้าประเภทไม่ถูกต้อง ไม่ถืออะไรเลย (ป้องกัน Null Reference)
                break;
        }

        // 3. ถ้ามีอาวุธใหม่ถูกกำหนด (ไม่ใช่ null) ให้สั่ง Equip
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Equip(); // เรียก Equip() ของอาวุธใหม่ (จะเปิด GameObject ของอาวุธ)
            Debug.Log($"Player equipped: {CurrentWeapon.WeaponName}");
        }
    }

    /// <summary>
    /// เมธอดนี้จะถูกเรียกจาก BasicCombat เพื่อสั่งให้ "อาวุธที่ถืออยู่ปัจจุบัน" ทำการโจมตี
    /// </summary>
    public void PerformAttack()
    {
        if (CurrentWeapon == null)
        {
            Debug.LogWarning("Cannot attack: No weapon equipped!");
            return;
        }

        // ตรวจสอบประเภทของอาวุธที่ถืออยู่ปัจจุบัน และเรียกเมธอดโจมตีที่เหมาะสม
        if (_currentWeaponType == WeaponType.Melee)
        {
            // ทำการ Cast CurrentWeapon ไปเป็น MeleeWeapon เพื่อเรียกเมธอดเฉพาะของดาบ
            MeleeWeapon mw = CurrentWeapon as MeleeWeapon;
            if (mw != null) // ตรวจสอบอีกครั้งเพื่อความปลอดภัย
            {
                mw.PerformMeleeAttack();
            }
        }
        else if (_currentWeaponType == WeaponType.Ranged)
        {
            // ทำการ Cast CurrentWeapon ไปเป็น RangedWeapon เพื่อเรียกเมธอดเฉพาะของปืน
            RangedWeapon rw = CurrentWeapon as RangedWeapon;
            if (rw != null) // ตรวจสอบอีกครั้งเพื่อความปลอดภัย
            {
                rw.PerformRangedAttack();
            }
        }
    }

    /// <summary>
    /// Property สำหรับให้ Script อื่นๆ ตรวจสอบว่าผู้เล่นกำลังถืออาวุธประเภทใดอยู่
    /// </summary>
    /// <returns>WeaponType ของอาวุธที่ถืออยู่ปัจจุบัน</returns>
    public WeaponType GetCurrentWeaponType()
    {
        return _currentWeaponType;
    }
}