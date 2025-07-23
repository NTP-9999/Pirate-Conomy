using UnityEngine;

public abstract class LivingThing : MonoBehaviour
{
    // พื้นฐานของชีวิต
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    // สถานะว่าตายหรือยัง
    public bool IsDead { get; private set; } = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        // เริ่มต้น
    }

    // ฟังก์ชันรับความเสียหาย
    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamageTaken(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันฟื้นฟูสุขภาพ
    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHeal(amount);
    }

    // ฟังก์ชันเรียกตอนโดนทำร้าย (ให้ override)
    protected virtual void OnDamageTaken(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage, current health: {currentHealth}");
    }

    // ฟังก์ชันเรียกตอนฟื้นฟู (ให้ override)
    protected virtual void OnHeal(float amount)
    {
        Debug.Log($"{gameObject.name} healed {amount} health, current health: {currentHealth}");
    }

    // ฟังก์ชันตาย
    protected virtual void Die()
    {
        IsDead = true;
        Debug.Log($"{gameObject.name} died.");

        OnDeath();
    }

    // ฟังก์ชันเรียกตอนตาย (ให้ override)
    public virtual void OnDeath()
    {
        // ตัวอย่าง: ปิด gameobject
        Destroy(gameObject);
    }
}
