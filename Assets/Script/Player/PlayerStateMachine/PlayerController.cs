using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject playerWeapon;

    // Movement parameters
    public float walkSpeed = 3f;
    public float runSpeed  = 6f;
    public float jumpHeight = 2f;
    public float gravity    = -9.81f;
    public float rollSpeed  = 10f;

    // Combat parameters
    public float attackRange  = 1.5f;
    public float attackDamage = 20f;
    public LayerMask enemyLayer;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isSkillLocked = false;

    
    private CharacterController characterController;
    public Animator animator;
    private Camera    mainCamera;
    private float     verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator            = GetComponent<Animator>();
        mainCamera          = Camera.main;

        CharacterStats.Instance.OnPlayerDamaged += OnPlayerDamagedHandler;
    }
    void Update()
    {
        SnapToGround();
    }

    /// <summary>
    /// Wrapper สำหรับ CharacterController.Move
    /// </summary>
    public void Move(Vector3 displacement)
    {
        characterController.Move(displacement);
    }

    /// <summary>
    /// สั่งกระโดด ถ้ากำลังอยู่บนพื้น
    /// </summary>
    public void Jump()
    {
        if (IsGrounded())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * attackRange,
            attackRange, enemyLayer);

        foreach (var col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                var lt = col.GetComponent<LivingThing>();
                if (lt != null) lt.TakeDamage(attackDamage);
            }
        }
    }
    void SnapToGround()
    {
        // ยิง Raycast ลงมาจากหัวตัวละครลงพื้น
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 1.5f))
        {
            // ถ้าพื้นต่ำกว่าหรือตื้นกว่าตัวละคร ให้สแน็ปลงไป
            float targetY = hit.point.y;
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetY, 20f * Time.deltaTime);
            transform.position = pos;
        }
    }

    /// <summary>
    /// คืนค่าสถานะว่าอยู่บนพื้นหรือไม่
    /// </summary>
    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    private IEnumerator HurtLockCoroutine(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    private void OnPlayerDamagedHandler(float damage)
    {
        StartCoroutine(HurtLockCoroutine(0.6f));
    }

    /// <summary>
    /// เรียกจาก MoveState (หรือ JumpState) เพื่ออัปเดตการเคลื่อนที่+แรงโน้มถ่วง
    /// </summary>
    public void HandleMovement()
    {
        // เช็คพื้นก่อนอื่น (เพื่อรีเซ็ต verticalVelocity เมื่อแตะพื้น)
        bool grounded = characterController.isGrounded;
        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // ให้ติดพื้นแน่น
        }

        // ถ้า lock movement ไว้ (เช่น ตอน Hurt) ก็ให้ gravity ทำงาน แต่ไม่รับ input
        if (!canMove)
        {
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            return;
        }

        // ดึง input เดิม
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        isRunning   = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        if (inputDir.magnitude >= 0.1f)
        {
            // คำนวณมุมและเคลื่อนที่ตามกล้อง
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                                + mainCamera.transform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 disp    = moveDir * speed + Vector3.up * verticalVelocity;

            characterController.Move(disp * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            // ไม่มี input ก็ปล่อยให้ gravity ทำงาน
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        // อัปเดต verticalVelocity กับ Animator Parameters
        verticalVelocity += gravity * Time.deltaTime;
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", grounded);
    }
    /// <summary>
    /// ล็อกการใช้ skill: ลดความเร็ว เดินอย่างเดียว ห้าม Jump/Run/Roll ฯลฯ
    /// </summary>
    public IEnumerator SkillLock(float duration, float speedMultiplier)
    {
        isSkillLocked = true;
        // เก็บค่าสปีดเดิม
        float prevWalk = walkSpeed;
        float prevRun  = runSpeed;

        // ลดความเร็วทั้งเดิน/วิ่ง
        walkSpeed *= speedMultiplier;
        runSpeed  = walkSpeed;  

        yield return new WaitForSeconds(duration);

        // คืนค่าสปีดเดิม
        walkSpeed = prevWalk;
        runSpeed  = prevRun;

        isSkillLocked = false;
    }
}
