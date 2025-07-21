using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{
    [Tooltip("Damage to apply when this hitbox hits the player")]
    public float damage = 0f;
    [HideInInspector] public bool alreadyHit = false;

    private void Awake()
    {
        // เริ่มต้นปิด collider ไว้ก่อน
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (alreadyHit == true) return;

            var stats = other.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
                alreadyHit = true;
            }
        }
    }

    /// <summary>
    /// เปิด/ปิด HitBox
    /// </summary>
    public void SetActive(bool on)
    {
        GetComponent<Collider>().enabled = on;
    }
}
