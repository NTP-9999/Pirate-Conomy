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

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isSkillLocked = false;
    [HideInInspector] public bool skipGroundSnap = false;
    [HideInInspector] public bool isParryActive = false;
    [SerializeField] private ThirdPersonCamera thirdPersonCam;


    private CharacterController characterController;
    public Animator animator;
    private Camera mainCamera;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

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
        if (skipGroundSnap)
            return;
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

        bool grounded = characterController.isGrounded;
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
        bool wantRun = Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude >= 0.1f;

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

        if (!canMove)
        {
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            return;
        }

        Vector3 moveDir = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            if (isFPS)
            {
                Vector3 camForward = mainCamera.transform.forward;
                Vector3 camRight = mainCamera.transform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                moveDir = camForward * inputDir.z + camRight * inputDir.x;

                transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
            }
            else
            {
                float cameraYaw = mainCamera.transform.eulerAngles.y;

    // หาทิศทางการเดินตาม input และกล้อง
                float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraYaw;
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                moveDir = moveDirection.normalized;

                // หันตัวละคร เฉพาะตอนมี input
                if (inputDir.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
        }

        // ✅ Gravity และการเคลื่อนที่ต้องอยู่รวมกัน
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 displacement = moveDir * speed + Vector3.up * verticalVelocity;
        characterController.Move(displacement * Time.deltaTime);

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
    public void RefreshCamera()
    {
        mainCamera = Camera.main;
    }
}
