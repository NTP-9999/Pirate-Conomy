using UnityEngine;
using System.Collections;

public class ParrySkill : ISkill
{
    public string Name => "Parry";
    public bool IsOnCooldown { get; private set; }
    public string ProjTag => _projTag;

    PlayerController _pc;
    ParryHitbox      _hitbox;
    float            _castDelay;      // รอให้ Animation เริ่ม (ก่อนเปิด window)
    float            _windowDuration; // ระยะเวลาที่รับ Parry
    float            _cooldown;
    string           _projTag;

    public ParrySkill(
        PlayerController pc,
        ParryHitbox hitbox,
        float castDelay,
        float windowDuration,
        float cooldown,
        string projTag
    )
    {
        _pc             = pc;
        _hitbox         = hitbox;
        _castDelay      = castDelay;
        _windowDuration = windowDuration;
        _cooldown       = cooldown;
        _projTag        = projTag;

        _hitbox.parrySkill = this;
    }

    public IEnumerator Activate()
    {
        IsOnCooldown = true;
        _pc.isParryActive = true;

        // เล่นอนิเมชัน Parry
        _pc.animator?.SetTrigger("Parry");

        // รอจังหวะจะเปิด Parry Window
        yield return new WaitForSeconds(_castDelay);

        _hitbox.Open();
        yield return new WaitForSeconds(_windowDuration);
        _hitbox.Close();
        _pc.isParryActive = false;

        // รอ Cooldown จบ
        yield return new WaitForSeconds(_cooldown);
        IsOnCooldown = false;
    }

    public void Reflect(Projectile proj)
    {
        // เปลี่ยนเจ้าของและสะท้อนกลับ
        proj.OnParried(_pc.transform, proj.owner);
        // TODO: ถ้ามี VFX/เสียงเพิ่มเติมสำหรับ Parry Success ให้เรียกที่นี่
    }
}
