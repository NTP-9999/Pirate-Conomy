using UnityEngine;

// LivingEntityStats.cs
public class LivingEntityStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float _currentHealth; // ใช้ SerializeField เพื่อให้เห็นใน Inspector
    public float maxHealth = 100f; // ค่า Max Health เริ่มต้น

    // Property สำหรับ Health ที่จะอัปเดตอัตโนมัติและควบคุมการตาย
    public float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth); // จำกัดค่า Health ให้อยู่ระหว่าง 0 ถึง maxHealth
            if (_currentHealth <= 0 && !IsDead) // ตรวจสอบว่า Health หมดและยังไม่ตาย
            {
                Die(); // เรียกเมธอด Die()
            }
        }
    }

    public bool IsDead { get; protected set; } = false; // สถานะการตาย (protected set เพื่อให้คลาสลูกแก้ไขได้)

    // เมธอด Start ที่สามารถถูก Override โดยคลาสลูกได้
    protected virtual void Start()
    {
        currentHealth = maxHealth; // กำหนด Health เริ่มต้นให้เท่ากับ Max Health
    }

    // เมธอดสำหรับรับความเสียหาย ที่สามารถถูก Override ได้
    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return; // ถ้าตายแล้ว ไม่รับดาเมจเพิ่ม
        currentHealth -= amount; // ลด Health
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");
    }

    // เมธอดสำหรับจัดการเมื่อตาย ที่สามารถถูก Override ได้
    protected virtual void Die()
    {
        if (IsDead) return; // ถ้าตายแล้ว ไม่ต้องทำซ้ำ
        IsDead = true; // ตั้งสถานะว่าตายแล้ว
        Debug.Log($"{gameObject.name} has been defeated!");
        Destroy(gameObject);
    }
}