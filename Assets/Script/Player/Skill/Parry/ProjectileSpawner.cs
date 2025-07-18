using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Tooltip("ลาก EnemyProjectile Prefab ของคุณลงที่นี่")]
    public GameObject projectilePrefab;
    [Tooltip("จุดปล่อยกระสุน (ตำแหน่งของ GameObject นี้)")]
    public Transform firePoint;
    [Tooltip("รอให้ยิงหลังเริ่มเล่นกี่วินาที")]
    public float delay = 1f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnOne), delay, delay);
    }

    void SpawnOne()
    {
        var player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("ไม่พบ Player (Tag=Player) ในฉาก!");
            return;
        }

        // สร้าง projectile
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var proj = go.GetComponent<Projectile>();
        if (proj == null)
        {
            Debug.LogError("Prefab นี้ไม่มีสคริปต์ Projectile!");
            return;
        }

        // คำนวณทิศทางยิงตรงไปหา player
        Vector3 dir = (player.position - firePoint.position).normalized;
        proj.Shoot(dir, transform);
    }
}
