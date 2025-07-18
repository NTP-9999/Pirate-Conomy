using UnityEngine;

// Require Collider ปรับเป็น Trigger และสั่ง Enable/Disable จาก ParrySkill
[RequireComponent(typeof(Collider))]
public class ParryHitbox : MonoBehaviour
{
    [HideInInspector] public ParrySkill parrySkill;
    Collider _col;

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
        _col.enabled = false;
    }

    /// <summary>
    /// เรียกเพื่อเปิดรับการชน projectile
    /// </summary>
    public void Open() => _col.enabled = true;

    /// <summary>
    /// เรียกเพื่อปิดไม่รับชน projectile
    /// </summary>
    public void Close() => _col.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        // เช็ค Tag ให้ตรงกับที่กำหนดใน ParrySkill
        if (!_col.enabled || !other.CompareTag(parrySkill.ProjTag)) return;

        var proj = other.GetComponent<Projectile>();
        if (proj != null)
        {
            // เรียกสะท้อนกลับผ่าน ParrySkill
            parrySkill.Reflect(proj);
        }
    }
}
