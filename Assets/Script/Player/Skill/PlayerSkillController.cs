using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    private Dictionary<string, ISkill> _skills;

    [Header("Firewall Skill")]
    public GameObject firewallPrefab;
    public float firewallOffset = 1f;
    public float firewallHeight = 0.5f;
    public float castingDelay   = 1f;
    public float lockDuration    = 3f;
    public float slowMultiplier  = 0.5f;
    public float cooldownTime    = 5f;

    [Header("Punch Skill")]
    public GameObject punchEffectPrefab;          // Prefab ของ Particle / VFX
    public Transform  punchEffectOrigin;          // Transform (ตำแหน่งปล่อย)
    public float      punchCastDelay    = 0.3f;   // เท่ากับความยาว animation
    public float      punchCooldown     = 1f;
    public float      punchRange        = 2f;
    public string     projectileTag     = "EnemyProjectile";

    [Header("Block Skill")]
    public float blockCastDelay    = 0.5f;  // ความยาวอนิเมชันบล็อค
    public float blockLockDuration = 0.5f;  // ระยะเวลาล็อกเดินไม่ได้ (มักเท่ากับ castDelay)
    public float blockCooldown     = 2f;    // คูลดาวน์สกิล

    [Header("Parry Skill")]
    public ParryHitbox parryHitbox;        // assign จาก Inspector (child ของ Player)
    public float      parryCastDelay      = 0.2f;
    public float      parryWindowDuration = 0.4f;
    public float      parryCooldown       = 2f;
    // ใช้ tag เดิมจาก PunchSkill ถ้าต้องการให้ Parry สะท้อนเฉพาะ EnemyProjectile
    // public string   projectileTag;

    void Awake()
    {
        var pc = GetComponent<PlayerController>();
        var cam = Camera.main;
        _skills = new Dictionary<string, ISkill>
        {
            ["Firewall"] = new FirewallSkill(
                pc,
                firewallPrefab,
                pc.transform,
                cam,
                castingDelay,
                lockDuration,
                slowMultiplier,
                cooldownTime,
                firewallOffset,
                firewallHeight
            ),
            ["Punch"] = new PunchSkill(
                pc,
                punchEffectPrefab,
                punchEffectOrigin,
                punchCastDelay,
                punchCooldown,
                punchRange,
                projectileTag
            ),
            ["Block"] = new BlockSkill(
                pc,
                blockCastDelay,
                blockLockDuration,
                blockCooldown
            ),
            ["Parry"] = new ParrySkill(
                pc,
                parryHitbox,
                parryCastDelay,
                parryWindowDuration,
                parryCooldown,
                "EnemyProjectile"
           )
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)
            && _skills["Firewall"].IsOnCooldown == false
            && SkillManager.Instance.IsUnlocked("Firewall"))
        {
            StartCoroutine(_skills["Firewall"].Activate());
        }
        if (Input.GetKeyDown(KeyCode.G)
            && !_skills["Punch"].IsOnCooldown
            && SkillManager.Instance.IsUnlocked("Punch"))
        {
            StartCoroutine(_skills["Punch"].Activate());
        }
        if (Input.GetKeyDown(KeyCode.T)
            && !_skills["Block"].IsOnCooldown
            && SkillManager.Instance.IsUnlocked("Block"))
        {
            StartCoroutine(_skills["Block"].Activate());
        }
        if (Input.GetKeyDown(KeyCode.R) // หรือปุ่มที่คุณเลือกสำหรับ Parry
            && !_skills["Parry"].IsOnCooldown
            && SkillManager.Instance.IsUnlocked("Parry"))
        {
            StartCoroutine(_skills["Parry"].Activate());
        }
    }
}
