using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AttackHitbox : MonoBehaviour
{
    [Tooltip("ค่าดาเมจที่จะส่ง")]
    public float damage = 10f;

    Collider col;
    HashSet<Collider> hitColliders = new HashSet<Collider>();

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    // เรียกจาก Animation Event แทน Activate()/Deactivate()
    public void ActivateOnePhysicsFrame()
    {
        // เคลียร์รายชื่อเป้าหมายเก่า
        hitColliders.Clear();
        StartCoroutine(ActivateFrame());
    }

    private IEnumerator ActivateFrame()
    {
        // 1) เปิด collider ทันที
        col.enabled = true;
        // 2) รอให้ Physics (FixedUpdate) วิ่งรอบหนึ่ง
        yield return new WaitForFixedUpdate();
        // 3) ปิด collider ทันทีหลังจากนั้น
        col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // ยิงดาเมจครั้งเดียวต่อ target ต่อการ Activate
        if (other.CompareTag("Player") && !hitColliders.Contains(other))
        {
            hitColliders.Add(other);
            CharacterStats.Instance?.TakeDamage(damage);
            Debug.Log($"Player took {damage} from {name}");
        }
    }
}
