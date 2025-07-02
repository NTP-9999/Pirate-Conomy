using UnityEngine;

public class RangedWeapon : Weapon // ตรวจสอบว่าสืบทอดจาก Weapon
{
    [Header("Ranged Weapon Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private Transform muzzlePoint; // จุดที่กระสุนจะถูกยิงออกมา (ปลายกระบอกปืน)
    [SerializeField] private LayerMask hitMask; // Layer ที่กระสุนจะชนได้ (เช่น Enemy, Boss)
    [SerializeField] private GameObject bulletPrefab;

    [Header("Visual & Audio Feedback")]
    [SerializeField] private GameObject muzzleFlashPrefab; // Prefab ของ Particle Effect แสงแฟลชที่ปลายปืน
    [SerializeField] private AudioClip shootSound; // เสียงยิง
    private AudioSource audioSource;

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

        // สั่งเล่น Animation ยิงปืน
        if (basicCombat != null && basicCombat.playerAnimator != null)
        {
            basicCombat.playerAnimator.SetTrigger("AttackRanged");
        }
    }

    public void PerformRangedAttackEvent()
    {
        Debug.Log($"{WeaponName} is firing (Animation Event triggered this)!");

        // 1. เล่นเสียง
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // 2. สร้าง muzzle flash
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(muzzleFlash, 0.5f);
        }

        // 3. ยิงกระสุนออกจากปลายปืน แต่พุ่งไปทางที่กล้องมอง
        if (bulletPrefab != null && muzzlePoint != null)
        {
            Transform cam = Camera.main.transform;

            // ยิง Ray ออกจากกลางหน้าจอไปในทิศทางกล้อง
            Ray ray = new Ray(cam.position, cam.forward);
            Vector3 targetPoint;

            // หาก Ray ชนอะไร เราจะยิงไปจุดนั้น
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = cam.position + cam.forward * 100f; // ยิงไปไกลลิบถ้าไม่ชน
            }

            // คำนวณทิศทางจาก muzzlePoint ไปยังจุดที่ Ray ชน
            Vector3 shootDirection = (targetPoint - muzzlePoint.position).normalized;

            GameObject bulletGO = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(shootDirection));
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(shootDirection);
                bulletScript.SetDamage(damage); // ส่งค่า damage จาก RangedWeapon ไปให้ Bullet
            }
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