using UnityEngine;
using UnityEngine.UI;
public class KravalonAI : MonoBehaviour
{
    [HideInInspector] public KravalonStateMachine StateMachine;
    [HideInInspector] public KravalonChaseState chaseState;
    [HideInInspector] public KravalonAttackState attackState;

    [Header("Health")]
    public float maxHealth = 200f;
    private float currentHealth;
    public Animator animator;

    public Transform shipTarget => ShipEnterExit.Instance?.transform;
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 3f;
    public float damage = 40f;
    public float stoppingDistance = 3f;
    public float rotationSpeed = 5f;
    [Header("Exit Settings")]
    [Tooltip("ระยะที่ Kravalon จะเลิกไล่เรือ และร่วงลงพื้น")]
    public float exitRange = 30f;
    [Header("Health Bar UI")]
    [Tooltip("GameObject Panel ที่เก็บ Image ของแถบเลือด")]
    public GameObject healthBarUI;
    [Tooltip("Image Type=Filled, Fill Method=Horizontal")]
    public Image     healthBarFill;

    private float lastAttackTime = -Mathf.Infinity;

    void Awake()
    {
        StateMachine = new KravalonStateMachine();
        chaseState = new KravalonChaseState(this);
        attackState = new KravalonAttackState(this);
        currentHealth = maxHealth;
    }

    void Start()
    {
        StateMachine.Initialize(chaseState);
        CameraShake.Instance.Shake(2f, 0.5f);
        // เริ่มจากไล่ก่อน
        
        KravalonAudioManager.Instance.PlayOneShot(KravalonAudioManager.Instance.spawnSFX);
    }
    

    void Update()
    {
        UpdateHealthBar();
        if (Vector3.Distance(transform.position, shipTarget.position) > exitRange)
        {
            ExitAndDespawn();
            return;
        }
        float dist = Vector3.Distance(transform.position, shipTarget.position);
        if (dist > exitRange)
        {
            // ซ่อน UI ก่อน
            if (healthBarUI != null)
                healthBarUI.SetActive(false);

            ExitAndDespawn();
            return;
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        RotateTowardsShip(Time.deltaTime);      // ← เพิ่มบรรทัดนี้
        StateMachine.Update();
    }

    
    private void UpdateHealthBar()
    {
        if (healthBarUI == null || healthBarFill == null || shipTarget == null)
            return;

        // แสดงเฉพาะเมื่ออยู่ใน exitRange
        float dist = Vector3.Distance(transform.position, shipTarget.position);
        bool inRange = dist <= exitRange;
        healthBarUI.SetActive(inRange);

        if (inRange)
        {
            // อัพเดต fillAmount ให้เป็นสัดส่วนเลือดปัจจุบัน
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }


    public bool IsShipInAttackRange()
    {
        return Vector3.Distance(transform.position, shipTarget.position) <= attackRange;
    }

    public bool IsAttackCooldownReady()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }
    public bool IsShipInStoppingDistance()
    {
        return Vector3.Distance(transform.position, shipTarget.position) <= stoppingDistance;
    }
    /// <summary>
    /// หมุน Kravalon ให้หันหน้าไปหาเรือ โดยล็อคแกน Y
    /// ทำงานทุกเฟรมก่อน State Logic
    /// </summary>
    public void RotateTowardsShip(float deltaTime)
    {
        if (shipTarget == null) return;
        // 1) หาทิศทาง
        Vector3 dir = shipTarget.position - transform.position;
        // 2) ล็อค Y
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        // 3) คำนวณ rotation เป้าหมาย
        Quaternion targetRot = Quaternion.LookRotation(dir);
        // 4) ค่อย ๆ หมุน
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            deltaTime * rotationSpeed     // rotationSpeed กำหนดใน KravalonAI
        );
    }
    public void SetAttackCooldown()
    {
        lastAttackTime = Time.time;
    }

    public void DealDamageToShip()
    {
        if (shipTarget.TryGetComponent<ShipStats>(out var stats))
        {
            stats.TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Kravalon โดนยิง! HP: {currentHealth}");
    }

    void Die()
    {
       
        animator.SetTrigger("Die");
        Debug.Log("💀 Kravalon ตายแล้ว!");
        Destroy(gameObject, 2f);
    }
    /// <summary>
    /// ให้ Kravalon ร่วงลงพื้นทันที (โดย Raycast หา ground) แล้วทำลายตัวเอง
    /// </summary>
    private void ExitAndDespawn()
    {
        // 1) Raycast ลงหาพื้น
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 p = transform.position;
            p.y = hit.point.y;
            transform.position = p;
        }

        // 2) (ออฟชันนอล) ปลด NavMeshAgent / หยุดอนิเมชั่น หลังกระแทก
        // var agent = GetComponent<NavMeshAgent>();
        // if (agent != null) agent.isStopped = true;
        // animator.SetTrigger("HitGround");

        // 3) ทำลายตัวเอง
        Destroy(gameObject);
    }
}
