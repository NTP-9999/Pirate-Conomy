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
        // เช็คว่าโดนมอนสเตอร์หรือศัตรู
        var stats = collision.gameObject.GetComponent<CharacterStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }

        Destroy(gameObject); // ทำลายกระสุนหลังชน
    }
}
