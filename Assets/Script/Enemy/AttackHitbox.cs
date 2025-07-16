using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [Tooltip("ค่าดาเมจที่จะส่ง")]
    public float damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ถ้ามีระบบ Stats อยู่
            CharacterStats.Instance?.TakeDamage(damage);
            Debug.Log($"Player took {damage} from {name}");
        }
    }

    // เตรียมให้ Animation Event เรียกตอนช่วง Active Frames
    public void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void Deactivate()
    {
        GetComponent<Collider>().enabled = false;
    }
}
