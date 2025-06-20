using UnityEngine;
using System.Collections.Generic; // เพื่อใช้ Dictionary

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Holder")]
    [Tooltip("The Transform where equipped weapons will be parented. This should be a child of your player character, usually near their hand.")]
    public Transform weaponHolder; // ตำแหน่งที่อาวุธจะถูกวางเมื่อสวมใส่

    [Header("Current Weapon State")]
    [SerializeField]
    private WeaponType _currentWeaponType = WeaponType.Melee; // กำหนดค่าเริ่มต้น
    public Weapon CurrentWeapon { get; private set; } // อ้างอิงถึง Instance ของอาวุธที่อยู่ใน Scene ตอนนี้

    // ใช้ Dictionary เพื่อเก็บอ้างอิงของ Component อาวุธแต่ละชนิด
    private Dictionary<WeaponType, Weapon> _weaponInstances = new Dictionary<WeaponType, Weapon>();

    void Awake()
    {
        if (weaponHolder == null)
        {
            Debug.LogError("PlayerWeaponManager: Weapon Holder Transform not assigned! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }

        // ค้นหา Component อาวุธจาก Child ของ WeaponHolder
        FindWeaponsInHolder();

        // ตรวจสอบว่ามีอาวุธเริ่มต้นที่เราต้องการ Equip หรือไม่
        if (_weaponInstances.ContainsKey(_currentWeaponType))
        {
            EquipWeapon(_currentWeaponType);
        }
        else
        {
            Debug.LogError($"PlayerWeaponManager: Initial weapon type ({_currentWeaponType}) not found in Weapon Holder. Please ensure the corresponding weapon Prefab is a child of Weapon Holder.", this);
            enabled = false; // ปิดสคริปต์ถ้าหาอาวุธเริ่มต้นไม่เจอ
        }
    }

    private void FindWeaponsInHolder()
    {
        _weaponInstances.Clear(); // เคลียร์ Dictionary ก่อนทุกครั้งที่ค้นหา

        // วนลูปผ่าน Child ทุกตัวของ weaponHolder
        foreach (Transform child in weaponHolder)
        {
            Weapon weapon = child.GetComponent<Weapon>(); // พยายามดึง Base Weapon Component

            if (weapon != null)
            {
                // ตรวจสอบว่าเป็น MeleeWeapon หรือ RangedWeapon
                if (weapon is MeleeWeapon meleeWeapon)
                {
                    _weaponInstances.Add(WeaponType.Melee, meleeWeapon);
                    meleeWeapon.Unequip(); // ตั้งค่าเริ่มต้นให้ซ่อนอาวุธไว้ก่อน
                }
                else if (weapon is RangedWeapon rangedWeapon)
                {
                    _weaponInstances.Add(WeaponType.Ranged, rangedWeapon);
                    rangedWeapon.Unequip(); // ตั้งค่าเริ่มต้นให้ซ่อนอาวุธไว้ก่อน
                }
                else
                {
                    Debug.LogWarning($"PlayerWeaponManager: Found a Weapon component '{weapon.name}' on child '{child.name}' but it's neither MeleeWeapon nor RangedWeapon. It will be ignored.", child);
                }
            }
        }

        if (!_weaponInstances.ContainsKey(WeaponType.Melee))
        {
            Debug.LogWarning("PlayerWeaponManager: No Melee Weapon found as child of Weapon Holder!", weaponHolder);
        }
        if (!_weaponInstances.ContainsKey(WeaponType.Ranged))
        {
            Debug.LogWarning("PlayerWeaponManager: No Ranged Weapon found as child of Weapon Holder!", weaponHolder);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentWeaponType != WeaponType.Melee && _weaponInstances.ContainsKey(WeaponType.Melee))
            {
                EquipWeapon(WeaponType.Melee);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentWeaponType != WeaponType.Ranged && _weaponInstances.ContainsKey(WeaponType.Ranged))
            {
                EquipWeapon(WeaponType.Ranged);
            }
        }
    }

    public void EquipWeapon(WeaponType typeToEquip)
    {
        // 1. ถ้ามีอาวุธที่ถืออยู่ปัจจุบัน ให้สั่ง Unequip ก่อน
        if (CurrentWeapon != null)
        {
            CurrentWeapon.Unequip(); // สั่งให้ GameObject ของอาวุธเก่า SetActive(false)
        }

        // 2. กำหนดอาวุธใหม่จาก Dictionary
        if (_weaponInstances.TryGetValue(typeToEquip, out Weapon newWeapon))
        {
            CurrentWeapon = newWeapon; // กำหนดอาวุธที่ถืออยู่เป็นอาวุธใหม่
            _currentWeaponType = typeToEquip; // อัปเดตประเภทอาวุธปัจจุบัน

            CurrentWeapon.Equip(); // เรียก Equip() ของอาวุธใหม่ (จะเปิด GameObject ของอาวุธ)
            Debug.Log($"Player equipped: {CurrentWeapon.WeaponName} ({CurrentWeapon.Type})");
        }
        else
        {
            Debug.LogWarning($"PlayerWeaponManager: Attempted to equip weapon type '{typeToEquip}' but it was not found in Weapon Holder's children.");
            CurrentWeapon = null; // ถ้าหาไม่เจอ ก็ไม่ถืออะไร
        }
    }

    // เมธอดนี้จะถูกเรียกจาก BasicCombat เพื่อสั่งให้ "อาวุธที่ถืออยู่ปัจจุบัน" ทำการโจมตี
    public void PerformAttack()
    {
        if (CurrentWeapon == null)
        {
            Debug.LogWarning("Cannot attack: No weapon equipped!");
            return;
        }

        // CurrentWeapon จะเป็น instance ที่ถูกต้องอยู่แล้ว เราเรียกเมธอดโจมตีได้เลย
        // แต่ถ้า PerformMeleeAttackEvent และ PerformRangedAttackEvent ไม่ได้อยู่ใน Weapon base class
        // คุณยังคงต้อง Cast เหมือนเดิม (ตามโค้ดที่คุณเคยให้มา)
        if (_currentWeaponType == WeaponType.Melee)
        {
            MeleeWeapon mw = CurrentWeapon as MeleeWeapon;
            if (mw != null)
            {
                mw.PerformMeleeAttackEvent();
            }
            else
            {
                Debug.LogError($"Current weapon is Melee type but not a MeleeWeapon component! {CurrentWeapon.name}", CurrentWeapon);
            }
        }
        else if (_currentWeaponType == WeaponType.Ranged)
        {
            RangedWeapon rw = CurrentWeapon as RangedWeapon;
            if (rw != null)
            {
                rw.PerformRangedAttackEvent();
            }
            else
            {
                Debug.LogError($"Current weapon is Ranged type but not a RangedWeapon component! {CurrentWeapon.name}", CurrentWeapon);
            }
        }
    }

    public WeaponType GetCurrentWeaponType()
    {
        return _currentWeaponType;
    }
}