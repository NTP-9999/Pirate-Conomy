using UnityEngine;

public class RangedWeapon : Weapon // ตรวจสอบว่าสืบทอดจาก Weapon
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

    void Awake() // เปลี่ยนเป็น protected override void Awake() ถ้า Weapon มี Awake() ด้วย
    {
        if (Type != WeaponType.Ranged)
        {
            Debug.LogWarning($"RangedWeapon script on {gameObject.name} has WeaponType set to {Type}. It should be WeaponType.Ranged.", this);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
                audioSource.volume = 0.5f;
            }
        }

        if (muzzlePoint == null)
        {
            muzzlePoint = transform;
            Debug.LogWarning($"RangedWeapon: Muzzle Point not assigned on {gameObject.name}. Using weapon's transform as muzzle point.", this);
        }
    }

    // เพิ่ม override Attack() เพื่อให้ BasicCombat เรียกใช้ได้
    public override void Attack()
    {
        // ตรวจสอบ Fire Rate ก่อน
        if (Time.time < nextFireTime)
        {
            Debug.Log($"{WeaponName}: Cannot fire yet. Cooldown remaining.");
            return; // ยังยิงไม่ได้
        }

        // ตรงนี้จะสั่งให้ playerAnimator เล่น Animation การยิงปืน
        if (basicCombat != null && basicCombat.playerAnimator != null)
        {
            basicCombat.playerAnimator.SetTrigger("Shoot"); // หรือใช้ SetInteger("WeaponType", (int)WeaponType.Ranged);
        }
        else
        {
            Debug.LogWarning("RangedWeapon: BasicCombat or playerAnimator not found. Cannot play shoot animation.", this);
        }
        
        nextFireTime = Time.time + fireRate; // ตั้งเวลา Cooldown ใหม่
        // Fire() จะถูกเรียกผ่าน Animation Event ในเวลาที่เหมาะสม
    }

    // เมธอดสำหรับสั่งยิงจริง (จะถูกเรียกจาก Animation Event)
    public void PerformRangedAttackEvent() // เปลี่ยนชื่อเป็น PerformRangedAttackEvent
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
            Destroy(muzzleFlash, 0.5f);
        }

        // 3. ยิง Raycast เพื่อตรวจจับการชน
        // ควรใช้ Transform ของ Player Camera เพื่อกำหนดทิศทางของ Raycast
        Transform playerCamera = Camera.main.transform; // สมมติว่ากล้องผู้เล่นคือ Camera.main
        if (playerCamera == null)
        {
            Debug.LogError("RangedWeapon: Main Camera not found. Cannot perform raycast.", this);
            return;
        }

        RaycastHit hit;
        // ยิง Raycast จากตำแหน่งกล้อง ไปในทิศทางที่กล้องมอง
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, Mathf.Infinity, hitMask))
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
            else if (hit.collider.CompareTag("Monster"))
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

    public override void Equip()
    {
        base.Equip();
    }

    public override void Unequip()
    {
        base.Unequip();
    }
}