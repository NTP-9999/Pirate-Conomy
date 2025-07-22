using UnityEngine;
using System.Collections;

public class PoisonVFX : MonoBehaviour
{
    [Header("Initial & DOT Damage")]
    public float initialDamage = 10f;
    public float dotDamage     = 2f;
    public float dotDuration   = 2f;
    public float dotInterval   = 1f;

    [Header("Target")]
    public LayerMask targetMask; // ตั้งให้ตรงกับ Layer ของ Player

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจว่าชนกับ Player ผ่าน LayerMask
        if (((1 << other.gameObject.layer) & targetMask) == 0) return;

        var stats = other.GetComponent<CharacterStats>();
        if (stats == null) return;

        // ทำดาเมจเบื้องต้น
        stats.TakeDamage(initialDamage);

        // เริ่ม DOT Coroutine
        StartCoroutine(DoDOT(stats));

        // ไม่ทำให้ VFX หายเองที่นี่ (จะปลดทีหลังด้วย Animation Event)
    }

    private IEnumerator DoDOT(CharacterStats stats)
    {
        float elapsed = 0f;
        while (elapsed < dotDuration)
        {
            yield return new WaitForSeconds(dotInterval);
            stats.TakeDamage(dotDamage);
            elapsed += dotInterval;
        }
    }
}
