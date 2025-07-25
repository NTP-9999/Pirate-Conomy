using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Cannon Points")]
    // สำหรับปืนใหญ่ฝั่งซ้าย (2 กระบอก)
    public Transform[] leftCannonPoints;
    // สำหรับปืนใหญ่ฝั่งขวา (2 กระบอก)
    public Transform[] rightCannonPoints;

    [Header("Cannonball Settings")]
    public GameObject cannonballPrefab;
    public float cannonballSpeed = 30f;
    public float cannonballDamage = 20f;

    void Update()
    {
        // ยิงกระสุนได้เฉพาะตอนที่ผู้เล่นกำลังควบคุมเรืออยู่
        if (!ShipEnterExit.Instance.isControlling) return;

        // กด Q เพื่อยิงปืนฝั่งซ้าย (ยิงจากทุกกระบอกใน Array)
        if (Input.GetKeyDown(KeyCode.Q))
            FireCannons(leftCannonPoints);

        // กด E เพื่อยิงปืนฝั่งขวา
        if (Input.GetKeyDown(KeyCode.E))
            FireCannons(rightCannonPoints);

        // กด Spacebar เพื่อยิงปืนทั้งสองฝั่งพร้อมกัน
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireCannons(leftCannonPoints);
            FireCannons(rightCannonPoints);
        }
    }

    void FireCannons(Transform[] cannonPoints)
    {
        if (cannonballPrefab == null || cannonPoints == null || cannonPoints.Length == 0) return;

        foreach (Transform firePoint in cannonPoints)
        {
            if (firePoint == null) continue;

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
