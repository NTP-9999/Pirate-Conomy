using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject playerWeapon;
    [Header("Camera Modes")]
    public bool isFPS = false;

    // Movement parameters
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rollSpeed = 10f;

    // Combat parameters
    public float attackRange = 1.5f;
    public float attackDamage = 20f;
    public LayerMask enemyLayer;

    [Header("Ground Snapping")]
    // ระบุเลเยอร์ของพื้น (Optional แต่แนะนำให้เซ็ตเฉพาะพื้นจะได้ไม่ชนกับ Collider อื่นๆ)
    public LayerMask groundLayer;
    // ยกจุดเริ่มยิง ray ขึ้นเหนือจุดยืนของตัวละคร
    public float groundCheckHeight = 1.0f;
    // ระยะลงไปหา ground สูงสุด
    public float groundCheckDistance = 2.0f;
    // ความเร็วในการสแน็ป (ยิ่งสูง ยิ่งเร็ว)
    public float snapSpeed = 10f;
    [Header("Stamina Costs")]
    public float jumpStaminaCost = 20f;
    public float rollStaminaCost = 15f;
    [Header("Roll Cooldown")]
    public float rollCooldown = 1f;      // เวลาระหว่าง roll แต่ละครั้ง
    private float rollCooldownTimer = 0f;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isSkillLocked = false;
    [HideInInspector] public bool skipGroundSnap = false;
    [HideInInspector] public bool isParryActive = false;
    [Header("Audio")]
    [HideInInspector] public PlayerAudioManager audioManager;


    private CharacterController characterController;
    public Animator animator;
    public Camera mainCamera;
    private float verticalVelocity;
    [Header("Ground Check (Improved)")]
    public float groundCheckOffset = 0.1f;  // ระยะเหนือพื้นสำหรับ Raycast
    public float groundCheckRadius = 0.3f;  // รัศมี SphereCast (ตรวจสอบพื้นแบบแม่นยำ)
    public float groundSnapThreshold = 0.5f; // ถ้าอยู่ใกล้พื้นขนาดนี้ ให้ snap ลงไป
    private bool isGroundedCached = false;
    private Vector3 groundHitPoint;

    private void Awake()
    {
        audioManager = GetComponent<PlayerAudioManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        CharacterStats.Instance.OnPlayerDamaged += OnPlayerDamagedHandler;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            attackDamage = 99999999999999f; // ทดสอบการโจมตีแรง
        }
        bool runPressed = Input.GetKey(KeyCode.LeftShift)
                   && Input.GetAxis("Vertical") > 0.1f;
        isRunning = runPressed;
        animator.SetBool("IsRunning", runPressed);
        SnapToGround();
        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= Time.deltaTime;
    }
    /// <summary>
    /// คืนค่าได้ว่า Roll ได้ไหม (Cooldown หมดแล้ว)
    /// </summary>
    public bool CanRoll()
    {
        return rollCooldownTimer <= 0f;
    }

    /// <summary>
    /// เริ่มนับ cooldown เมื่อกด Roll
    /// </summary>
    public void TriggerRollCooldown()
    {
        rollCooldownTimer = rollCooldown;
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
            Debug.Log("Jump Triggered"); // เช็คว่าถูกเรียกจริงไหม
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
                var snake = col.GetComponent<SnakeNagaAI>();
                if (snake != null) snake.TakeDamage(attackDamage);
                var wolf = col.GetComponent<WolfAI>();
                if (wolf != null) wolf.TakeDamage(attackDamage);
                var crab = col.GetComponent<CrabAI>();
                if (crab != null) crab.TakeDamage(attackDamage);
            }
            else if (col.CompareTag("Boss"))
            {
                var boss = col.GetComponent<BossStat>();
                if (boss != null) boss.TakeDamage(attackDamage);
            }
        }
    }
    void SnapToGround()
    {
        if (skipGroundSnap) return;

        bool grounded = IsGrounded();

        if (grounded)
        {
            float distanceToGround = transform.position.y - groundHitPoint.y;
            if (distanceToGround > 0.01f && distanceToGround < groundSnapThreshold)
            {
                characterController.Move(Vector3.down * distanceToGround);
            }
        }
    }

    /// <summary>
    /// คืนค่าสถานะว่าอยู่บนพื้นหรือไม่
    /// </summary>
    // ตรวจสอบว่าผู้เล่นแตะพื้นหรือไม่
        public bool IsGrounded()
        {
            Vector3 origin = transform.position + Vector3.up * groundCheckOffset;
            Ray ray = new Ray(origin, Vector3.down);

            bool hit = Physics.SphereCast(ray, groundCheckRadius, out RaycastHit hitInfo, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

            Debug.DrawRay(origin, Vector3.down * groundCheckDistance, hit ? Color.green : Color.red);
            Debug.Log("IsGrounded() = " + hit);

            if (hit)
            {
                isGroundedCached = true;
                groundHitPoint = hitInfo.point;
                return true;
            }

            isGroundedCached = false;
            return false;
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
        if (DialogueManager.IsInDialogue) return;
        var stats = CharacterStats.Instance;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        bool wantRun = Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude > 0.1f;
        float speed = walkSpeed;

        if (!isSkillLocked && wantRun && stats.currentStamina > 0f)
        {
            isRunning = true;
            stats.StartStaminaDrain();
            speed = runSpeed;
        }
        else
        {
            isRunning = false;
            stats.StopStaminaDrain();
        }

        if (stats.currentStamina <= 0f)
        {
            isRunning = false;
            speed = walkSpeed;
            stats.StopStaminaDrain();
        }

        // อัปเดตแรงโน้มถ่วงเสมอ
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 moveDir = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDir = camForward * inputDir.z + camRight * inputDir.x;
        }

        Vector3 displacement = moveDir * speed + Vector3.up * verticalVelocity;
        characterController.Move(displacement * Time.deltaTime);

        // Reset vertical velocity เมื่อแตะพื้น
        if (IsGrounded() && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        if (IsGrounded())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
            {
                if (hit.collider.CompareTag("Ship"))
                {
                    // ถ้ายังไม่ใช่ลูกของเรือ ให้ตั้ง Parent
                    if (transform.parent != hit.collider.transform)
                        transform.SetParent(hit.collider.transform, true);
                }
                else
                {
                    // ไม่ใช่พื้นเรือ ให้ถอด Parent และรีเซ็ต scale
                    if (transform.parent != null)
                        transform.SetParent(null, true);
                    
                    transform.localScale = Vector3.one;
                }
            }
            else
            {
                // ไม่ชนพื้นเลย ให้ถอด Parent และรีเซ็ต scale
                if (transform.parent != null)
                    transform.SetParent(null, true);

                transform.localScale = Vector3.one;
            }
        }
        else
        {
            // กรณีที่ไม่ได้ Grounded (กำลังลอยหรือกระโดด)
            if (transform.parent != null)
                transform.SetParent(null, true);

            transform.localScale = Vector3.one;
        }

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", IsGrounded());
    }


    /// <summary>
    /// ล็อกการใช้ skill: ลดความเร็ว เดินอย่างเดียว ห้าม Jump/Run/Roll ฯลฯ
    /// </summary>
    public IEnumerator SkillLock(float duration, float speedMultiplier)
    {
        isSkillLocked = true;
        // เก็บค่าสปีดเดิม
        float prevWalk = walkSpeed;
        float prevRun = runSpeed;

        // ลดความเร็วทั้งเดิน/วิ่ง
        walkSpeed *= speedMultiplier;
        runSpeed = walkSpeed;

        yield return new WaitForSeconds(duration);

        // คืนค่าสปีดเดิม
        walkSpeed = prevWalk;
        runSpeed = prevRun;

        isSkillLocked = false;
    }
}
