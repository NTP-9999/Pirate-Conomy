using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    private float time = 0f; // ใช้สำหรับตรวจสอบเวลา
    [HideInInspector] public Transform owner;    // เจ้าของปัจจุบัน (Boss ก่อน Parry, Player หลัง Parry)
    public float speed = 10f;
    public int   damage = 10;

    private Rigidbody    _rb;
    private Transform    _target;               // เป้าหมายที่ projectile จะตาม
    private Collider     _col;

    void Awake()
    {
        _rb  = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        // เราใช้ Trigger เพื่อเช็คการชนง่ายๆ
        _col.isTrigger = true;
    }

    /// <summary>
    /// เรียกเมื่อ Boss ยิงกระสุน
    /// </summary>
    /// <param name="direction">ทิศทางยิงครั้งแรก</param>
    /// <param name="shooter">Transform ของผู้ยิง (Boss)</param>
    public void Shoot(Vector3 direction, Transform shooter)
    {
        owner = shooter;
        // ตั้งเป้าเป็น Player (หาโดย Tag="Player")
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) _target = go.transform;

        // ตั้งทิศทางและความเร็วเริ่มต้น
        transform.forward = direction.normalized;
        _rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >= 4f)
        {
            Destroy(gameObject);
        }
        // ถ้ามีเป้า ให้คำนวณทิศทางและปรับ velocity ทุกเฟรม
        if (_target != null)
        {
            Vector3 dir = (_target.position - transform.position).normalized;
            transform.forward = dir;
            _rb.linearVelocity = dir * speed;
        }
    }

    /// <summary>
    /// เรียกเมื่อถูก Parry
    /// </summary>
    /// <param name="parryUser">ผู้ Parry (Player)</param>
    /// <param name="newTarget">เป้าหมายใหม่ (Boss)</param>
    public void OnParried(Transform parryUser, Transform newTarget)
    {
        owner   = parryUser;
        _target = newTarget;  // เปลี่ยนเป้าไปตามที่ ParrySkill บอก

        // คุณจะเห็นใน Update ว่ามันจะหมุนตาม _target เอง
    }

    void OnTriggerEnter(Collider other)
    {
        // 1) ถ้าชน Player (ก่อน Parry)
        if (other.CompareTag("Player") && owner != other.transform)
        {
            // ทำดาเมจให้ Player
            CharacterStats.Instance.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Boss") && owner != other.transform)
        {
            var BossStat = other.GetComponent<BossStat>();
            if (BossStat != null) BossStat.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // 3) ถ้าโดนสิ่งอื่น (ผนัง ฯลฯ) ก็ทำลายได้ตามใจ
        // Destroy(gameObject);
    }
}
