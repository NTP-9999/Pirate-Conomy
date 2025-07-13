using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterStats))]
public class CharacterMovement : MonoBehaviour
{
    public Vector3 PlayerPosition
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
    private CharacterController controller;
    private Animator animator;
    private CharacterStats stats;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runMultiplier = 1.5f;
    public float gravity = 20f;
    public float jumpHeight = 2f; // <<< เพิ่ม: ความสูงในการกระโดด

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
        
        // ถ้ากำลังกลิ้งอยู่ ให้หยุดการทำงานที่เหลือเพื่อไม่ให้เคลื่อนที่หรือกระโดด
        if (isRolling)
        {
            controller.Move(moveDirection * Time.deltaTime); // ยังคงให้กลิ้งต่อไป
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
            animator.SetBool("IsGrounded", true);

            // *** เพิ่ม: การกระโดด ***
            if (Input.GetButtonDown("Jump") && canMove) // "Jump" คือ Spacebar โดย default
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
                animator.SetTrigger("JumpTrigger"); // เรียก Animator Trigger
                animator.SetBool("IsGrounded", false); // ตั้งเป็น false ทันทีเมื่อเริ่มกระโดด
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
            animator.SetBool("IsGrounded", false);
        }

        Vector3 desiredMove = transform.forward * input.z + transform.right * input.x;
        moveDirection.x = desiredMove.x * currentSpeed;
        moveDirection.z = desiredMove.z * currentSpeed;
        
        controller.Move(moveDirection * Time.deltaTime);

        // Animator parameters
        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputZ", input.z);
        animator.SetFloat("Speed", isRunning ? 2f : isMoving ? 1f : 0f);
    }

    void StopMovementAnimation()
    {
        animator.SetFloat("InputX", 0f);
        animator.SetFloat("InputZ", 0f);
        animator.SetFloat("Speed", 0f);
    }

    void HandleDodgeRollInput(Vector3 currentInputMovement)
    {
        // เพิ่มเงื่อนไข !isYLocked เพื่อไม่ให้กลิ้งเมื่อ Y ถูกล็อค
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextRollTime && !isRolling && !stats.isDead && canMove && !isYLocked)
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
        canMove = false; // ป้องกันการเคลื่อนที่ปกติระหว่าง Roll

        // ตั้งค่า moveDirection.y ให้เป็น 0 เพื่อไม่ให้ได้รับผลจากแรงโน้มถ่วงขณะกลิ้ง
        moveDirection.y = 0; 

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
        // เรียกใช้ Coroutine เพื่อจบสถานะการกลิ้งหลังจากระยะเวลาที่กำหนด
        StartCoroutine(StopDodgeRollAfterDuration());
    }

    System.Collections.IEnumerator StopDodgeRollAfterDuration()
    {
        yield return new WaitForSeconds(rollDuration);
        isRolling = false;
        canMove = true; // เปิดให้เคลื่อนที่ปกติได้
        // รีเซ็ต moveDirection ในแกน XZ เพื่อป้องกันการเคลื่อนที่ต่อเนื่องจากการกลิ้ง
        moveDirection.x = 0;
        moveDirection.z = 0;
        Debug.Log("Dodge roll finished.");
    }

    public void ResetMovementDirection()
    {
        moveDirection = Vector3.zero;
        Debug.Log("Movement direction reset.");
    }

    public void ForceStopCharacterController()
    {
        moveDirection.x = 0;
        moveDirection.z = 0;
        Debug.Log("CharacterController forced to stop horizontal movement.");
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
        if (!state)
        {
            ForceStopCharacterController();
        }
    }

    public void LockYPosition()
    {
        isYLocked = true;
        lockedYPosition = transform.position.y;
        Debug.Log($"Player Y position Locked at: {lockedYPosition}");
    }

    public void UnlockYPosition()
    {
        isYLocked = false;
        if (controller.isGrounded)
        {
            moveDirection.y = -0.5f;
        }
        else
        {
            moveDirection.y = 0f;
        }
        Debug.Log("Player Y position Unlocked!");
    }
}