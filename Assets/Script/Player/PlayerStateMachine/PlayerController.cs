using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController characterController;
    public Animator animator;
    private Camera mainCamera;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    private bool isRunning;

    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    private float verticalVelocity;
    public float attackRange = 1.5f;
    public float attackDamage = 20f;
    public LayerMask enemyLayer;

    private bool grounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        grounded = characterController.isGrounded;

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // ให้ติดพื้น
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            Vector3 moveDirRotated = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            characterController.Move((moveDirRotated * currentSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
        else
        {
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Animator
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", grounded);
    }

    public void Jump()
    {
        if (grounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    public void DealDamage()
    {
        // สร้างกล่องเช็คชนิด Collider รอบ player
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange, attackRange, enemyLayer);

        foreach (Collider enemyCollider in hitEnemies)
        {
            // เช็คว่า object นั้นมี tag เป็น Enemy หรือไม่
            if (enemyCollider.CompareTag("Enemy"))
            {
                // หา component LivingThing (หรือ Test) แล้วเรียก TakeDamage
                LivingThing enemyLivingThing = enemyCollider.GetComponent<LivingThing>();
                if (enemyLivingThing != null)
                {
                    enemyLivingThing.TakeDamage(attackDamage);
                }
            }
        }
    }

    public bool IsGrounded()
    {
        return grounded;
    }
    IEnumerator WaitEndAttack()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Collider>().enabled = false;
    }
}

