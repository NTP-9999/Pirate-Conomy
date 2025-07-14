// ใน PlayerSkillController.cs
using UnityEngine;
using System.Collections;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Firewall Skill")]
    public GameObject firewallPrefab;
    public float      firewallOffset    = 1f;
    public float      firewallHeight    = 0.5f;
    public float      castDelay         = 1f;   // เวลาร่ายก่อนยิง
    public float      lockDuration      = 3f;   // ระยะเวลาล็อก action
    public float      speedMultiplier   = 0.5f; // เดินช้าลง 50%
    public float      cooldown          = 5f;   // Cooldown ทั้งหมด

    public Transform playerTransform;
    public Camera    playerCamera;

    private Animator animator;
    private bool     onCooldown = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) &&
            !onCooldown &&
            SkillManager.Instance.IsUnlocked("Firewall"))
        {
            StartCoroutine(CastingFirewall());
        }
    }

    IEnumerator CastingFirewall()
    {
        onCooldown = true;

        // 1) Lock player action + ลดสปีด
        var pc = playerTransform.GetComponent<PlayerController>();
        StartCoroutine(pc.SkillLock(lockDuration, speedMultiplier));

        // 2) Trigger animation (ถ้ามี)
        if (animator != null)
            animator.SetTrigger("CastFirewall");

        // 3) รอจน animation จบ (castDelay)
        yield return new WaitForSeconds(castDelay);

        // 4) ยิงจริง
        CastFirewall();

        // 5) รอจนหมด cooldown
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    void CastFirewall()
    {
        // ทิศกล้องบนระนาบ XZ
        Vector3 dir = playerCamera.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        // จุด Spawn หน้าผู้เล่น
        Vector3 origin = playerTransform.position
                       + dir * firewallOffset
                       + Vector3.up * firewallHeight;

        // สร้าง projectile
        var go = Instantiate(firewallPrefab, origin, Quaternion.identity);
        go.GetComponent<FirewallProjectile>().Initialize(dir);
    }
}
