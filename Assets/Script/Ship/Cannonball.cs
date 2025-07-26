using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float damage = 20f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // ป้องกันค้างในฉาก
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // เช็คว่ามีคอมโพเนนต์ CharacterStats หรือไม่
            var stats = collision.gameObject.GetComponent<KravalonAI>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }

            Destroy(gameObject); // ทำลายกระสุนหลังชนศัตรู
        }
    }
}
