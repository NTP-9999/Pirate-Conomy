using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Cannon Points")]
    // สำหรับปืนใหญ่ฝั่งซ้าย (2 กระบอก)
    public Transform[] leftCannonPoints;
    // สำหรับปืนใหญ่ฝั่งขวา (2 กระบอก)
    public Transform[] rightCannonPoints;
    public Transform[] frontCannonPoints; // สำหรับปืนใหญ่ด้านหน้า (ถ้ามี)

    [Header("Cannonball Settings")]
    public GameObject cannonballPrefab;
    public float cannonballSpeed = 30f;
    public float cannonballDamage = 20f;
    [Header("Fire Cooldown")]
    [Tooltip("เวลาที่ต้องรอหลังยิงก่อนยิงซ้ำ (วินาที)")]
    public float fireCooldown = 1f;
    private float lastFireTime = -Mathf.Infinity;
    private float lastLeftFireTime = 2.5f;
    [Header("Effects")]
    [Tooltip("ParticleSystem Prefab ของประกายปากลำกล้อง")]
    public GameObject muzzleFlashPrefab;
    [Tooltip("เสียงยิงปืน")]
    public AudioClip fireSfx;
    [Tooltip("AudioSource ถ้าอยากยิงผ่าน AudioSource แทน PlayClipAtPoint")]
    public AudioSource audioSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            cannonballDamage = 9999f;
            Debug.Log("🔥 cannonballDmg! = " + cannonballDamage );
        }
        if (Time.time < lastFireTime + fireCooldown)
            return;

        bool didFire = false;
        // ยิงกระสุนได้เฉพาะตอนที่ผู้เล่นกำลังควบคุมเรืออยู่
        if (!ShipEnterExit.Instance.isControlling) return;

        // กด Q เพื่อยิงปืนฝั่งซ้าย (ยิงจากทุกกระบอกใน Array)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FireCannons(leftCannonPoints);
            didFire = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            FireCannons(rightCannonPoints);
            didFire = true;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FireCannons(frontCannonPoints);
            didFire = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastLeftFireTime + fireCooldown)
        {
            FireCannons(leftCannonPoints);
            FireCannons(rightCannonPoints);
            FireCannons(frontCannonPoints);
            lastLeftFireTime = Time.time;
        }

        if (didFire)
        {
            // บันทึกเวลาที่ยิงล่าสุด
            lastFireTime = Time.time;
        }
    }
    

    void FireCannons(Transform[] cannonPoints)
    {
        if (cannonballPrefab == null || cannonPoints == null || cannonPoints.Length == 0) return;

        if (fireSfx != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(fireSfx);
            else
                AudioSource.PlayClipAtPoint(fireSfx, transform.position);
        }

        foreach (Transform firePoint in cannonPoints)
        {
            if (firePoint == null) continue;

            if (muzzleFlashPrefab != null)
                Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);

            GameObject cannonball = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = cannonball.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = firePoint.forward * cannonballSpeed;

            Cannonball cannonballScript = cannonball.GetComponent<Cannonball>();
            if (cannonballScript != null)
                cannonballScript.damage = cannonballDamage;
        }
    }
}
