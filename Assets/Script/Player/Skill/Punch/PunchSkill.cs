using UnityEngine;
using System.Collections;

public class PunchSkill : ISkill
{
    public string Name => "Punch";
    public bool IsOnCooldown { get; private set; }

    // refs & params
    private readonly PlayerController _pc;
    private readonly GameObject       _punchEffectPrefab;
    private readonly Transform        _origin;       // ตำแหน่งปล่อย VFX
    private readonly float            _castDelay;    // ระยะเวลาก่อนใช้สกิล (เท่ากับความยาว animation)
    private readonly float            _lockDuration; // เวลา lockdown (ล็อกเดินไม่ได้)
    private readonly float            _cooldown;     // เวลาคูลดาวน์นับถอยหลัง
    private readonly float            _range;        // ระยะตรวจหา projectiles
    private readonly string           _projTag;      // Tag ของเป้าหมายที่จะทำลาย
    private readonly float _damage; // ดาเมจที่ทำ


    public PunchSkill(
        PlayerController pc,
        GameObject punchEffectPrefab,
        Transform origin,
        float castDelay,
        float cooldown,
        float range,
        float damage, // ดาเมจที่ทำ
        string projTag
    )
    {
        _pc = pc;
        _punchEffectPrefab = punchEffectPrefab;
        _origin = origin;
        _castDelay = castDelay;
        _lockDuration = castDelay;  // ล็อกเดินไม่ได้จน animation จบ
        _cooldown = cooldown;
        _range = range;
        _damage = damage;
        _projTag = projTag;
    }

    public IEnumerator Activate()
    {
        IsOnCooldown = true;

        // ── ซ่อนอาวุธก่อนสกิล ───────────────────────
        if (_pc.playerWeapon != null)
            _pc.playerWeapon.SetActive(false);

        // ── ล็อกการเคลื่อนที่ ────────────────────────
        _pc.StartCoroutine(_pc.SkillLock(_lockDuration, 0f));

        // ── รัน Animation ────────────────────────────
        _pc.animator?.SetTrigger("Punch");

        // ── รอจบ Animation (castDelay) ───────────────
        yield return new WaitForSeconds(_castDelay);

        CameraShake.Instance.Shake(0.3f, 1f);
        

        // ── ปล่อย VFX + ทำลาย projectile ───────────
        SpawnEffect();
        DamageEnemies();
        DestroyProjectilesByTag();
        yield return new WaitForSeconds(1.2f);
        if (_pc.playerWeapon != null)
            _pc.playerWeapon.SetActive(true);

        // ── รอ Cooldown ──────────────────────────────
        yield return new WaitForSeconds(_cooldown);
        IsOnCooldown = false;
    }
    private void DamageEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(
            _origin.position,
            _range,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide
        );
        foreach (var c in hits)
        {
            if (c.CompareTag("Enemy"))
            {
                var living = c.GetComponent<Stats>();
                if (living != null)
                {
                    living.TakeDamage(_damage);
                    Debug.Log($"Hit {c.name}: dealt {_damage} damage");
                }
            }
            if (c.CompareTag("Boss"))
            {
                var living = c.GetComponent<BossStat>();
                if (living != null)
                {
                    living.TakeDamage(_damage);
                    Debug.Log($"Hit {c.name}: dealt {_damage} damage");
                }
            }
        }
    }

    private void SpawnEffect()
    {
        // ให้เอฟเฟคหันไปตาม forward ของ player
        Quaternion rot = Quaternion.LookRotation(_pc.transform.forward, Vector3.up);
        var go = Object.Instantiate(
            _punchEffectPrefab,
            _origin.position,
            rot
        );
        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
    }

    private void DestroyProjectilesByTag()
    {
        Collider[] hits = Physics.OverlapSphere(_origin.position, _range);
        foreach (var c in hits)
            if (c.CompareTag(_projTag))
                Object.Destroy(c.gameObject);
    }
}
