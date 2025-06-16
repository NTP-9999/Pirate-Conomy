using UnityEngine;
// ต้องมี using TMPro; ถ้าคุณใช้ TextMeshPro
// using TMPro; // ถ้าใช้ TextMeshPro ใน UIManager

public class BossStats : MonoBehaviour
{
    [SerializeField] private float _currentHealth; // เปลี่ยนเป็น private float และใช้ SerializeField เพื่อให้เห็นใน Inspector
    public float maxHealth = 500f; // ตัวอย่าง Max Health ของ Boss
    public string bossName = "PATRIXXX"; // เพิ่มชื่อ Boss

    // ************************************************************
    // เพิ่ม Reference สำหรับ WinScreenManager
    [Header("Win Screen")]
    public WinScreenManager winScreenManager; 
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

            // เรียก UIManager เพื่ออัปเดต UI ของ Boss
            if (UIManager.instance != null)
            {
                UIManager.instance.UpdateBossHealth(_currentHealth, maxHealth);
            }

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
        currentHealth = maxHealth; // กำหนดค่าเริ่มต้นผ่าน Property เพื่อให้ UI อัปเดตทันที

        // แสดง UI ของ Boss เมื่อ Boss ปรากฏตัวครั้งแรก (หรือเมื่อ GameObject นี้ Active)
        if (UIManager.instance != null)
        {
            UIManager.instance.ShowBossUI(bossName); // ส่งชื่อ Boss ไปด้วย
            UIManager.instance.UpdateBossHealth(currentHealth, maxHealth); // อัปเดตค่าเริ่มต้น
        }

        // ************************************************************
        // ตรวจสอบและหา WinScreenManager ถ้ายังไม่ได้ Assign ใน Inspector
        if (winScreenManager == null)
        {
            winScreenManager = FindObjectOfType<WinScreenManager>();
            if (winScreenManager == null)
            {
                Debug.LogError("WinScreenManager not found in scene! Win screen will not activate on boss defeat.", this);
            }
        }
        // ************************************************************
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

        // ซ่อน UI ของ Boss เมื่อ Boss ตาย
        if (UIManager.instance != null)
        {
            UIManager.instance.HideBossUI();
        }

        // ************************************************************
        // เรียก WinScreenManager เพื่อเริ่มลำดับหน้าจอ "คุณชนะ"
        if (winScreenManager != null)
        {
            winScreenManager.OnPlayerWin(); 
        }
        // ************************************************************

        // ใส่ Logic อื่นๆ เมื่อ Boss ตายที่นี่ (เช่น เล่นแอนิเมชันตาย, ทำลาย GameObject)
        Destroy(gameObject, 3f); // ทำลาย GameObject หลังจาก 3 วินาที (ให้เวลาเล่นแอนิเมชัน/เอฟเฟกต์)
    }
}