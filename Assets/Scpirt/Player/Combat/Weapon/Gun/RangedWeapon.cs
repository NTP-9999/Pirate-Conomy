using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float fireRate = 0.5f; // เวลาหน่วงระหว่างการยิง (วินาที)
    [SerializeField] private Transform muzzlePoint; // จุดที่กระสุนจะถูกยิงออกมา (ปลายกระบอกปืน)
    [SerializeField] private LayerMask hitMask; // Layer ที่กระสุนจะชนได้ (เช่น Enemy, Boss)

    [Header("Visual & Audio Feedback")]
    [SerializeField] private GameObject muzzleFlashPrefab; // Prefab ของ Particle Effect แสงแฟลชที่ปลายปืน
    [SerializeField] private AudioClip shootSound; // เสียงยิง
    private AudioSource audioSource; // สำหรับเล่นเสียง

    private float nextFireTime = 0f;

    void Awake()
    {
        // ตรวจสอบให้แน่ใจว่าได้ตั้งค่า WeaponType เป็น Ranged
        if (Type != WeaponType.Ranged)
        {
            Debug.LogWarning($"RangedWeapon script on {gameObject.name} has WeaponType set to {Type}. It should be WeaponType.Ranged.", this);
        }

        // พยายามหา AudioSource หากยังไม่ได้ลากใส่ใน Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // ถ้าไม่มี AudioSource ให้เพิ่มเข้าไป
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // ทำให้เป็น 3D sound
                audioSource.volume = 0.5f;
            }
        }

        // ตรวจสอบ muzzlePoint
        if (muzzlePoint == null)
        {
            // ถ้าไม่มี muzzlePoint ให้ใช้ตำแหน่งของ GameObject ตัวเอง
            muzzlePoint = transform;
            Debug.LogWarning($"RangedWeapon: Muzzle Point not assigned on {gameObject.name}. Using weapon's transform as muzzle point.", this);
        }
    }

    // เมธอดสำหรับสั่งยิง
    public void PerformRangedAttack()
    {
        Fire();
    }

    void Fire()
    {
        Debug.Log($"{WeaponName} is firing (Animation Event triggered this)!");

        // 1. เล่นเสียง
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // 2. สร้าง Muzzle Flash (ถ้ามี)
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(muzzleFlash, 0.5f); // ทำลาย effect หลังจาก 0.5 วินาที
        }

        // 3. ยิง Raycast เพื่อตรวจจับการชน
        Transform cameraTransform = Camera.main.transform; // สมมติว่ากล้องผู้เล่นคือ Camera.main

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, hitMask))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");

            // ตรวจสอบ Tag เพื่อทำดาเมจกับ Boss หรือ Enemy (มอนสเตอร์เล็ก)
            if (hit.collider.CompareTag("Boss"))
            {
                BossStats bossStats = hit.collider.GetComponent<BossStats>();
                if (bossStats != null && !bossStats.IsDead)
                {
                    bossStats.TakeDamage(damage);
                    Debug.Log($"{WeaponName} hit Boss for {damage} damage. Remaining health: {bossStats.currentHealth}");
                }
            }
            else if (hit.collider.CompareTag("Enemy")) // มอนสเตอร์เล็ก
            {
                LivingEntityStats enemyStats = hit.collider.GetComponent<LivingEntityStats>();
                if (enemyStats != null && !enemyStats.IsDead)
                {
                    enemyStats.TakeDamage(damage);
                    Debug.Log($"{WeaponName} hit Enemy {hit.collider.name} for {damage} damage. Remaining health: {enemyStats.currentHealth}");
                }
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
        }
    }
    
    // Override Equip และ Unequip เหมือน MeleeWeapon
    public override void Equip()
    {
        base.Equip(); // เรียก Equip ของคลาสแม่ (เปิด GameObject)
    }

    public override void Unequip()
    {
        base.Unequip(); // เรียก Unequip ของคลาสแม่ (ปิด GameObject)
    }
}