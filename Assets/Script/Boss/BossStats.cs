using UnityEngine;
// ต้องมี using TMPro; ถ้าคุณใช้ TextMeshPro
// using TMPro; // ถ้าใช้ TextMeshPro ใน UIManager

public class BossStats : MonoBehaviour
{
    [SerializeField] private float _currentHealth; // เปลี่ยนเป็น private float และใช้ SerializeField เพื่อให้เห็นใน Inspector
    public float maxHealth = 500f; // ตัวอย่าง Max Health ของ Boss
    public string bossName = "PATRIXXX"; 
    // ************************************************************

    // Property สำหรับ Health ที่จะอัปเดต UI อัตโนมัติ
    public float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (_currentHealth < 0) _currentHealth = 0;
            if (_currentHealth > maxHealth) _currentHealth = maxHealth;
            // ถ้า Boss ตาย
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public bool IsDead { get; private set; } = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount) // เปลี่ยนจาก int เป็น float เพื่อให้สอดคล้องกับ BasicCombat.cs (attackDamage เป็น float)
    {
        if (IsDead) return;
        currentHealth -= amount; // ใช้ Property setter
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log(bossName + " has been defeated!");
        Destroy(gameObject, 3f); // ทำลาย GameObject หลังจาก 3 วินาที (ให้เวลาเล่นแอนิเมชัน/เอฟเฟกต์)
    }
}