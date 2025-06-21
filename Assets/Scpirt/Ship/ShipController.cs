using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float turnSpeed = 30f;
    public float waterDrag = 0.5f;

    private Rigidbody rb;
    private float currentForwardInput = 0f;
    private float currentTurnInput = 0f;
    private ShipEnterExit enterExit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyDrag();
        HandleMovement();
    }

    void HandleMovement()
    {
        if (enterExit == null || !enterExit.IsControlling) return;
        currentForwardInput = Input.GetAxis("Vertical");   // W/S
        currentTurnInput = Input.GetAxis("Horizontal");    // A/D

        // เดินหน้า/ถอยหลัง
        Vector3 force = transform.forward * currentForwardInput * acceleration;
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(force, ForceMode.Acceleration);
        }

        // หมุนเรือ
        if (Mathf.Abs(currentTurnInput) > 0.1f)
        {
            float turn = currentTurnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion rotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * rotation);
        }
    }

    void ApplyDrag()
    {
        // ทำให้เรือช้าลงเมื่อไม่ได้เร่ง
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 dragForce = -horizontalVelocity * waterDrag;
        rb.AddForce(dragForce, ForceMode.Acceleration);
    }
}

