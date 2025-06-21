using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BasicCombat))]
[RequireComponent(typeof(CharacterStats))]
public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private BasicCombat combat;
    private CharacterStats stats;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runMultiplier = 1.5f;
    public float gravity = 20f;

    [Header("General Movement Control")]
    public bool canMove = true; // ควบคุมการเคลื่อนที่ XZ (เดิน/วิ่ง)
    public bool isYLocked = false; // <<< เพิ่ม: ควบคุมการล็อค Y-axis
    private float lockedYPosition; // <<< เพิ่ม: ตำแหน่ง Y ที่ถูกล็อค

    [Header("Dodge Roll Settings")]
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;
    public float rollStaminaCost = 30f;
    public float rollCooldown = 1f;

    [HideInInspector] public bool isRolling = false;

    private Vector3 moveDirection = Vector3.zero;
    private bool isRunning = false;
    private float nextRollTime = 0f;
    public static CharacterMovement Instance;

    void Start()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        combat = GetComponent<BasicCombat>();
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        if (stats.isDead) return;

        // *** Logic การล็อค Y-axis ***
        if (isYLocked)
        {
            // บังคับให้ตำแหน่ง Y ของผู้เล่นอยู่ที่ lockedYPosition เสมอ
            // และไม่ให้มีแรงโน้มถ่วงหรือการกระโดดมาเกี่ยวข้อง
            transform.position = new Vector3(transform.position.x, lockedYPosition, transform.position.z);
            // ถ้า canMove เป็น false ด้วย ก็จะไม่เกิดการเคลื่อนที่ XZ
            if (!canMove)
            {
                 StopMovementAnimation(); // หยุดแอนิเมชันการเคลื่อนที่
                 return; // หยุดการทำงานของ Update ที่เหลือ
            }
        }

        // ถ้า canMove เป็น false (แต่ Y ไม่ได้ถูกล็อค) หรือ Y ถูกล็อคอยู่แล้ว
        if (!canMove && !isYLocked) // !isYLocked เพื่อให้โค้ดส่วนบนทำงานก่อนถ้า Y ถูกล็อค
        {
            StopMovementAnimation(); // หยุดแอนิเมชันการเคลื่อนที่ทันที
            // ยังคงให้แรงโน้มถ่วงทำงานได้ ถ้า canMove เป็น false แต่ไม่ได้ล็อค Y
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            return;
        }

        // ดึง Input ดิบสำหรับ Animator ก่อน
        float hRaw = Input.GetAxisRaw("Horizontal");
        float vRaw = Input.GetAxisRaw("Vertical");

        animator.SetFloat("InputX", hRaw);
        animator.SetFloat("InputZ", vRaw);

        HandleMovement();
        HandleDodgeRollInput(new Vector3(hRaw, 0, vRaw));
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);
        if (input.magnitude > 1f) input.Normalize();

        bool isGrounded = controller.isGrounded;
        bool isMoving = input.magnitude > 0.1f;
        bool isForward = v > 0.1f;
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        isRunning = isMoving && isForward && shift && stats.currentStamina > 0;
        float currentSpeed = isRunning ? walkSpeed * runMultiplier : walkSpeed;

        if (isRunning)
        {
            stats.StartStaminaDrain();
        }
        else
        {
            stats.StopStaminaDrain();
        }

        if (isGrounded)
        {
            moveDirection.y = -0.5f; // ค่าเล็กน้อยเพื่อให้ CharacterController ติดพื้น
            // อัปเดต animator parameter "IsGrounded" เมื่ออยู่บนพื้น
            animator.SetBool("IsGrounded", true); // <<< แก้ไข: ตั้งเป็น true เมื่ออยู่บนพื้น
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
            animator.SetBool("IsGrounded", false); // <<< แก้ไข: ตั้งเป็น false เมื่อไม่อยู่บนพื้น
        }

        Vector3 desiredMove = transform.forward * input.z + transform.right * input.x;
        moveDirection.x = desiredMove.x * currentSpeed;
        moveDirection.z = desiredMove.z * currentSpeed;
        
        controller.Move(moveDirection * Time.deltaTime);

        // Animator parameters
        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputZ", input.z);
        animator.SetFloat("Speed", isRunning ? 2f : isMoving ? 1f : 0f);
        // animator.SetBool("IsGrounded", isGrounded); // ย้ายไปอยู่ใน block isGrounded/else
    }

    void StopMovementAnimation()
    {
        animator.SetFloat("InputX", 0f);
        animator.SetFloat("InputZ", 0f);
        animator.SetFloat("Speed", 0f);
    }

    void HandleDodgeRollInput(Vector3 currentInputMovement)
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextRollTime && !isRolling  && !stats.isDead && canMove) // เพิ่ม && canMove
        {
            if (stats.currentStamina >= rollStaminaCost)
            {
                DoDodgeRoll(currentInputMovement);
                stats.UseStamina(rollStaminaCost);
                nextRollTime = Time.time + rollCooldown;
            }
            else
            {
                Debug.Log("Not enough stamina for roll!");
            }
        }
    }

    void DoDodgeRoll(Vector3 rollInput)
    {
        isRolling = true;
        animator.SetTrigger("Roll");

        if (rollInput.magnitude > 0.1f)
        {
            Vector3 rollDirection = transform.forward * rollInput.z + transform.right * rollInput.x;
            moveDirection.x = rollDirection.normalized.x * rollSpeed;
            moveDirection.z = rollDirection.normalized.z * rollSpeed;
            Debug.Log($"Rolling in direction: {rollDirection.normalized}");
        }
        else
        {
            moveDirection.x = transform.forward.x * rollSpeed;
            moveDirection.z = transform.forward.z * rollSpeed;
            Debug.Log("Rolling forward.");
        }
        moveDirection.y = 0; // ในระหว่าง Roll ไม่ควรได้รับผลจากแรงโน้มถ่วงหรือกระโดด
    }

    public void ResetMovementDirection()
    {
        moveDirection = Vector3.zero;
        Debug.Log("Movement direction reset.");
    }

    public void ForceStopCharacterController()
    {
        // เมื่อ canMove เป็น false เราจะหยุดการเคลื่อนที่ XZ ผ่านการ return ใน Update
        // moveDirection.x และ moveDirection.z จะไม่ถูกอัปเดตถ้า !canMove
        // แต่เพื่อความมั่นใจว่าความเร็วจะถูกเซ็ตเป็น 0 ทันที
        moveDirection.x = 0;
        moveDirection.z = 0;
        Debug.Log("CharacterController forced to stop horizontal movement.");
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
        if (!state)
        {
            // เมื่อไม่สามารถเคลื่อนที่ได้ (canMove = false) เรายังคงต้องให้แรงโน้มถ่วงทำงาน
            // ยกเว้นตอนที่ isYLocked เป็น true
            ForceStopCharacterController(); // หยุดการเคลื่อนที่ XZ
        }
    }

    // <<< เพิ่ม: เมธอดสำหรับล็อคและปลดล็อค Y-axis
    public void LockYPosition()
    {
        isYLocked = true;
        lockedYPosition = transform.position.y; // บันทึกตำแหน่ง Y ปัจจุบัน
        // เมื่อ Y ถูกล็อค ไม่ว่าผู้เล่นจะอยู่บนพื้นหรือไม่ จะถูกตรึงที่ lockedYPosition
        Debug.Log($"Player Y position Locked at: {lockedYPosition}");
    }

    public void UnlockYPosition()
    {
        isYLocked = false;
        // เมื่อปลดล็อค Y, ให้แรงโน้มถ่วงกลับมาทำงาน
        // ตรวจสอบว่าอยู่บนพื้นหรือไม่เพื่อกำหนด verticalVelocity เริ่มต้น
        if (controller.isGrounded)
        {
            moveDirection.y = -0.5f;
        }
        else
        {
            // ถ้าอยู่ในอากาศ ให้เริ่มคำนวณแรงโน้มถ่วงต่อ
            // (หรือจะ set เป็น 0 แล้วให้แรงโน้มถ่วงดึงลงมาก็ได้)
            moveDirection.y = 0f; // หรือค่าที่เหมาะสม
        }
        Debug.Log("Player Y position Unlocked!");
    }
}