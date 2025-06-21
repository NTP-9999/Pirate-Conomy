using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask enemyLayers = -1; // Layer ของศัตรูที่จะรับดาเมจ (default = ทุก layer)

    private Vector3 direction;
    private float damage = 0f; // ความเสียหายที่จะได้รับจาก RangedWeapon

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    // เมธอดสำหรับกำหนดดาเมจจาก RangedWeapon
    public void SetDamage(float bulletDamage)
    {
        damage = bulletDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // กระสุนหายหลัง 5 วิ
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit: " + other.name);

        // ตรวจสอบว่าวัตถุที่ชนอยู่ใน enemyLayers หรือไม่
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // ลองหา LivingEntityStats component
            LivingEntityStats targetStats = other.GetComponent<LivingEntityStats>();
            
            if (targetStats != null)
            {
                // ทำดาเมจ
                targetStats.TakeDamage(damage);
                Debug.Log($"Bullet dealt {damage} damage to {other.name}!");
            }
            else
            {
                Debug.Log($"Hit {other.name} but no LivingEntityStats found - no damage dealt.");
            }
        }

        // ทำลายกระสุนเมื่อชนอะไรก็ตาม
        Destroy(gameObject);
    }
}