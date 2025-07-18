using UnityEngine;
using System.Collections;

public class BlockSkill : ISkill
{
    public string Name => "Block";
    public bool IsOnCooldown { get; private set; }

    // refs & params
    private readonly PlayerController _pc;
    private readonly float            _castDelay;    // ระยะเวลาที่เล่นอนิเมชันบล็อค
    private readonly float            _lockDuration; // ระยะเวลาล็อกเดินไม่ได้
    private readonly float            _cooldown;     // คูลดาวน์ก่อนใช้ซ้ำ

    public BlockSkill(
        PlayerController pc,
        float castDelay,
        float lockDuration,
        float cooldown
    )
    {
        _pc           = pc;
        _castDelay    = castDelay;
        _lockDuration = lockDuration;
        _cooldown     = cooldown;
    }

    public IEnumerator Activate()
    {
        IsOnCooldown = true;

        // 2) ล็อกการเคลื่อนที่ (สปีด = 0) ตลอดช่วงบล็อค
        _pc.StartCoroutine(_pc.SkillLock(_lockDuration, 0f));

        // 3) สั่ง Animator เล่น Trigger "Block"
        _pc.animator?.SetTrigger("Block");
        CharacterStats.Instance.SetInvincibility(true);

        // 4) รอจนอนิเมชันบล็อคจบ
        yield return new WaitForSeconds(_castDelay);
        CharacterStats.Instance.SetInvincibility(false);

        // 6) รอคูลดาวน์ก่อนให้ใช้สกิลได้อีกครั้ง
        yield return new WaitForSeconds(_cooldown);
        IsOnCooldown = false;
    }
}
