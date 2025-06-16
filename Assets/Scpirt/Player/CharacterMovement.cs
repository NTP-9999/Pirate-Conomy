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

    // *******************************************************************
    // ลบ Reference สำหรับ ThirdPersonCamera ออก เพราะตอนนี้ Camera จะจัดการการหมุน Player เอง
    // [Header("Camera Reference")]
    // public ThirdPersonCamera thirdPersonCamera; // ลาก GameObject ของกล้องที่มี ThirdPersonCamera Script มาใส่ใน Inspector

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runMultiplier = 1.5f;
    public float gravity = 20f;
    // public float rotationSpeed = 500f; // ลบตัวแปร rotationSpeed ออก

    // *******************************************************************
    // เพิ่มตัวแปรสำหรับควบคุมการเคลื่อนที่ (สำหรับระบบ Parry)
    [Header("General Movement Control")]
    public bool canMove = true; // ควบคุมว่าผู้เล่นสามารถรับ Input การเคลื่อนที่ได้หรือไม่
    // *******************************************************************

    [Header("Dodge Roll Settings")]
    public float rollSpeed = 10f;
    public float rollDuration = 0.5f;
    public float rollStaminaCost = 30f;
    public float rollCooldown = 1f;

    [HideInInspector] public bool isRolling = false;

    private Vector3 moveDirection = Vector3.zero;
    private bool isRunning = false;
    private float nextRollTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        combat = GetComponent<BasicCombat>();
        stats = GetComponent<CharacterStats>();

        // *******************************************************************
        // ลบโค้ดตรวจสอบ ThirdPersonCamera ออก
        // if (thirdPersonCamera == null)
        // {
        //     thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();
        //     if (thirdPersonCamera == null)
        //     {
        //         Debug.LogError("ThirdPersonCamera not found or assigned! Player rotation might not work correctly.");
        //     }
        // }
        // *******************************************************************
    }

    void Update()
    {
        if (stats.isDead) return;

        // *******************************************************************
        // เพิ่มเงื่อนไข canMove ตรงนี้
        // หากตัวละครไม่สามารถเคลื่อนที่ได้ (เช่น กำลัง Parry)
        // เราจะไม่ประมวลผล Input การเคลื่อนที่ และจะหยุดแอนิเมชันการเคลื่อนที่
        if (!canMove)
        {
            StopMovementAnimation(); // หยุดแอนิเมชันการเคลื่อนที่ทันที
            // ในที่นี้ เราไม่ต้องการให้มีการเคลื่อนที่ใดๆ ในสถานะ canMove = false
            // ยกเว้น Dodge Roll ซึ่งมีการจัดการแยกกัน
            return; // ออกจาก Update() ทันที ไม่ต้องประมวลผลการเคลื่อนที่หรือ Roll
        }
        // *******************************************************************


        // ดึง Input ดิบสำหรับ Animator ก่อน
        float hRaw = Input.GetAxisRaw("Horizontal");
        float vRaw = Input.GetAxisRaw("Vertical");

        animator.SetFloat("InputX", hRaw); // ใช้ Input ดิบสำหรับแอนิเมชัน
        animator.SetFloat("InputZ", vRaw); // Unity "V" คือ Z-axis

        // ************** ลำดับการเช็คสถานะที่ถูกต้อง **************
        // หากตัวละครกำลังโจมตี หรือ กำลังกลิ้ง ให้หยุดการเคลื่อนที่ปกติ
        if (combat.isAttacking || isRolling)
        {
            StopMovementAnimation(); // หยุดแอนิเมชันการเคลื่อนที่
            return; // ออกจาก Update() ทันที
        }
        // **********************************************************

        // หากไม่กำลังโจมตี ไม่กำลังกลิ้ง ไม่กำลัง Parry -> จัดการการเคลื่อนที่ปกติ
        HandleMovement();
        // ส่ง input ดิบไปให้ HandleDodgeRollInput เพื่อกำหนดทิศทางการกลิ้ง
        HandleDodgeRollInput(new Vector3(hRaw, 0, vRaw)); 
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // ใช้ GetAxis ปกติสำหรับแอนิเมชันเดิน/วิ่ง
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);
        if (input.magnitude > 1f) input.Normalize();

        bool isGrounded = controller.isGrounded;
        bool isMoving = input.magnitude > 0.1f;
        bool isForward = v > 0.1f;
        bool shift = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightShift);

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

        // *******************************************************************
        // ลบส่วนการหมุนตัวละครตามทิศทางของกล้องออกไปทั้งหมด
        // การหมุนจะถูกจัดการใน ThirdPersonCamera.cs แทน
        // if (thirdPersonCamera != null && !thirdPersonCamera.IsLockedOn)
        // {
        //     float mouseX = Input.GetAxis("Mouse X");
        //     transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
        //
        //     if (isMoving)
        //     {
        //         Vector3 camForward = thirdPersonCamera.GetCameraForward();
        //         Vector3 camRight = thirdPersonCamera.GetCameraRight();
        //         Vector3 relativeInput = camForward * input.z + camRight * input.x;
        //         
        //         if (relativeInput.magnitude > 0.1f)
        //         {
        //             Quaternion targetRotation = Quaternion.LookRotation(relativeInput.normalized);
        //             transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //         }
        //     }
        // }
        // *******************************************************************

        if (isGrounded)
        {
            // คำนวณ moveDirection ใหม่ทุกเฟรมหากอยู่บนพื้น
            // สำคัญ: ต้องใช้ transform.forward/right ของ Player เอง หลังจาก Player หมุนแล้ว
            Vector3 desiredMove = transform.forward * input.z + transform.right * input.x;
            moveDirection.x = desiredMove.x * currentSpeed;
            moveDirection.z = desiredMove.z * currentSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        // Animator parameters
        animator.SetFloat("InputX", input.x); 
        animator.SetFloat("InputZ", input.z);
        animator.SetFloat("Speed", isRunning ? 2f : isMoving ? 1f : 0f);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void StopMovementAnimation()
    {
        animator.SetFloat("InputX", 0f);
        animator.SetFloat("InputZ", 0f);
        animator.SetFloat("Speed", 0f);
    }

    void HandleDodgeRollInput(Vector3 currentInputMovement)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextRollTime && !isRolling && !combat.isAttacking && !stats.isDead)
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

    // แก้ไข DoDodgeRoll ให้รับทิศทางการกลิ้ง
    void DoDodgeRoll(Vector3 rollInput)
    {
        isRolling = true;
        animator.SetTrigger("Roll");

        // กำหนดทิศทางการกลิ้งตาม Input ที่เข้ามาสัมพันธ์กับทิศทาง Player ปัจจุบัน (ซึ่งถูกหมุนโดยกล้องแล้ว)
        if (rollInput.magnitude > 0.1f) // ไม่ต้องอ้างอิง thirdPersonCamera ตรงนี้แล้ว
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
        moveDirection.y = 0;
    }

    public void ResetMovementDirection()
    {
        moveDirection = Vector3.zero;
        Debug.Log("Movement direction reset.");
    }

    public void ForceStopCharacterController()
    {
        if (controller.enabled)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
        }
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
}