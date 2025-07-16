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

    [Header("Ground Snapping")]
    // ระบุเลเยอร์ของพื้น (Optional แต่แนะนำให้เซ็ตเฉพาะพื้นจะได้ไม่ชนกับ Collider อื่นๆ)
    public LayerMask groundLayer;
    // ยกจุดเริ่มยิง ray ขึ้นเหนือจุดยืนของตัวละคร
    public float groundCheckHeight   = 1.0f;
    // ระยะลงไปหา ground สูงสุด
    public float groundCheckDistance = 2.0f;
    // ความเร็วในการสแน็ป (ยิ่งสูง ยิ่งเร็ว)
    public float snapSpeed           = 10f;
    [Header("Stamina Costs")]
    public float jumpStaminaCost = 20f;
    public float rollStaminaCost = 15f;

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
        bool runPressed = Input.GetKey(KeyCode.LeftShift)
                   && Input.GetAxis("Vertical") > 0.1f;
        isRunning = runPressed;
        animator.SetBool("IsRunning", runPressed);
        SnapToGround();
    }

    /// <summary>
    /// Wrapper สำหรับ CharacterController.Move
    /// </summary>
    public void Move(Vector3 displacement)
    {
        if (DialogueManager.IsInDialogue) return;
        characterController.Move(displacement);
    }

    /// <summary>
    /// สั่งกระโดด ถ้ากำลังอยู่บนพื้น
    /// </summary>
    public void Jump()
    {
        if (DialogueManager.IsInDialogue) return;
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
        // 1) เริ่ม raycast จากบนหัวลงมา
        Vector3 origin = transform.position + Vector3.up * groundCheckHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            // 2) หาระยะต่างระดับ y ระหว่างตัวละครกับพื้น
            float yOffset = hit.point.y - transform.position.y;
            if (Mathf.Abs(yOffset) > 0.01f)
            {
                // 3) เลื่อนผ่าน CharacterController.Move ให้ไปในทิศทางขึ้น/ลง
                //    โดยใช้ Lerp เพื่อความนุ่มนวล
                float moveY = Mathf.Lerp(0, yOffset, snapSpeed * Time.deltaTime);
                characterController.Move(Vector3.up * moveY);
            }
        }
        // ถ้า Raycast ไม่ชน (ตกลงน้ำ หรือตกเหว) ก็ปล่อยให้ gravity เดิมทำงานต่อได้
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
    var stats = CharacterStats.Instance;

    if (DialogueManager.IsInDialogue) return;

    // 1) Ground check เพื่อรีเซ็ต verticalVelocity
    bool grounded = characterController.isGrounded;
    if (grounded && verticalVelocity < 0f)
        verticalVelocity = -2f;

    // 2) อ่าน input ก่อน — ต้องประกาศ moveX, moveZ, inputDir, speed ก่อนใช้งาน
    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");
    Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
    bool wantRun = Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude >= 0.1f;

    // default speed เป็นเดิน
    float speed = walkSpeed;

    // 3) Stamina + running logic
    if (!isSkillLocked && wantRun && stats.currentStamina > 0f)
    {
        isRunning = true;
        stats.StartStaminaDrain();    // เริ่ม drain stamina
        speed = runSpeed;
    }
    else
    {
        isRunning = false;
        stats.StopStaminaDrain();     // หยุด drain แล้วเริ่ม regen ตาม delay
    }

    // บังคับถ้า Stamina หมด
    if (stats.currentStamina <= 0f)
    {
        isRunning = false;
        speed = walkSpeed;
        stats.StopStaminaDrain();
    }

    // 4) ถ้าโดน lock movement (เช่น ตอนโดนฮิต) ก็ปล่อย gravity แต่ไม่รับ input เดิน/วิ่ง
    if (!canMove)
    {
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        return;
    }

    // 5) เคลื่อนที่ปกติ ถ้ามี input
    if (inputDir.magnitude >= 0.1f)
    {
        // หมุนตัวและ move ตามกล้อง
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                            + mainCamera.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Vector3 disp    = moveDir * speed + Vector3.up * verticalVelocity;

        characterController.Move(disp * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }
    else
    {
        // ไม่มี input → แค่ปล่อย gravity
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // 6) อัพเดต verticalVelocity + Animator params
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
