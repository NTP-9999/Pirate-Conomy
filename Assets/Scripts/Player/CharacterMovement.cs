using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Animator animator;
    CharacterController characterController;
    CharacterStats characterStats; // <--- เพิ่มบรรทัดนี้

    [Header("Movement Settings")]
    public float speed = 6.0f; // ความเร็วในการเดิน
    public float runSpeedMultiplier = 1.5f; // ตัวคูณความเร็วเมื่อวิ่ง
    public float jumpSpeed = 7.5f; // ความสูงในการกระโดด
    public float gravity = 20.0f; // แรงโน้มถ่วง

    private Vector3 moveDirection = Vector3.zero;
    private bool isCurrentlyRunning = false; 

    void Start()
    {
        Time.timeScale = 1;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        characterStats = GetComponent<CharacterStats>(); 

        // ตรวจสอบว่า Components จำเป็นอยู่หรือไม่
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name + "! Please add an Animator component.");
        }
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on " + gameObject.name + "! Please add a Character Controller component.");
        }
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats component not found on " + gameObject.name + "! Please add a CharacterStats component.");
        }
    }

    void Update()
    {
        if (characterStats != null && characterStats.isDead)
        {
            // ถ้าตัวละครตายแล้ว ให้หยุดการทำงานทั้งหมดของ Update method นี้ทันที
            // ปล่อยให้ CharacterStats และ Animator จัดการแอนิเมชันตายและสถานะอื่นๆ เอง
            return; 
        }

        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");   

        bool isGrounded = characterController.isGrounded; 

        Vector3 inputVector = new Vector3(horizontalInput, 0, verticalInput);
        if (inputVector.magnitude > 1f)
        {
            inputVector.Normalize(); 
        }

        float currentSpeed = speed; 
        bool isMoving = inputVector.magnitude > 0.01f; 
        bool isMovingForward = verticalInput > 0.1f; 
        bool isPressingRun = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        // ตรวจสอบ Stamina สำหรับการวิ่ง
        // สามารถวิ่งได้ถ้ากดปุ่มวิ่ง, กำลังเดินหน้า, กำลังเคลื่อนที่, และมี Stamina เหลือ
        bool canRun = false;
        if (characterStats != null) // <--- เพิ่ม null check ก่อนเข้าถึง characterStats
        {
            canRun = isPressingRun && isMovingForward && isMoving && characterStats.currentStamina > 0;
        }

        // Logic การจัดการ Stamina Drain/Regen
        if (canRun && !isCurrentlyRunning) // เริ่มวิ่ง
        {
            isCurrentlyRunning = true;
            if (characterStats != null) characterStats.StartStaminaDrain(); // <--- เพิ่ม null check
        }
        else if (!canRun && isCurrentlyRunning) // หยุดวิ่ง
        {
            isCurrentlyRunning = false;
            if (characterStats != null) characterStats.StopStaminaDrain(); // <--- เพิ่ม null check
        }
        // ถ้า stamina หมดขณะวิ่ง ให้หยุด drain ใน CharacterStats แล้วจะ regen เอง

        // กำหนด currentSpeed สำหรับ CharacterController.Move (ความเร็วที่แท้จริงของตัวละครในโลก 3D)
        if (canRun) 
        {
            currentSpeed *= runSpeedMultiplier; 
        }

        if (isGrounded) 
        {
            Vector3 calculatedMoveDirection = transform.forward * inputVector.z + transform.right * inputVector.x;
            
            moveDirection.x = calculatedMoveDirection.x * currentSpeed; 
            moveDirection.z = calculatedMoveDirection.z * currentSpeed; 
            moveDirection.y = -1.0f; 

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                if (animator != null)
                {
                    animator.SetTrigger("JumpTrigger"); 
                }
            }
        }
        else 
        {
            Vector3 calculatedMoveDirection = transform.forward * inputVector.z + transform.right * inputVector.x;
            moveDirection.x = calculatedMoveDirection.x * speed * 0.5f; 
            moveDirection.z = calculatedMoveDirection.z * speed * 0.5f;
        }

        moveDirection.y -= gravity * Time.deltaTime; 
        characterController.Move(moveDirection * Time.deltaTime); 

        // ------------------------------------------
        // ควบคุม Animator Parameters 
        // ------------------------------------------
        if (animator != null)
        {
            animator.SetFloat("InputX", inputVector.x); 
            
            float animInputZ = inputVector.z; 
            float animSpeedParam = inputVector.magnitude; 

            if (canRun) 
            {
                animSpeedParam = 2.0f; 
                animInputZ = 1.0f; 
            }
            else if (isMoving) 
            {
                animSpeedParam = 1.0f; 
                animInputZ = inputVector.z; 
            }
            else 
            {
                animSpeedParam = 0.0f;
                animInputZ = 0.0f;
            }
            animator.SetFloat("InputZ", animInputZ); 
            animator.SetFloat("Speed", animSpeedParam);  
            animator.SetBool("IsGrounded", isGrounded); 
        }
    }
}