using UnityEngine;
public class ShipController : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float turnSpeed = 30f;
    public float waterDrag = 0.5f;

    private Rigidbody rb;
    private float currentForwardInput = 0f;
    private float currentTurnInput = 0f;
    private ShipEnterExit enterExit;

    private float yRotation; // ✅ ต้องย้ายมาที่นี่
    private float fixedYValue;

    void Start()
    {
        Vector3 currentEuler = transform.eulerAngles;
        yRotation = currentEuler.y; // เก็บ rotation Y ตอนเริ่ม
        fixedYValue = transform.position.y;

        enterExit = GetComponent<ShipEnterExit>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 🔒 ล็อค Rotation X/Z แต่รักษา Y
        transform.rotation = Quaternion.Euler(-4.696f, yRotation, 0f);

        // 🔒 ล็อคตำแหน่ง Y
        Vector3 position = transform.position;
        position.y = fixedYValue;
        transform.position = position;

        ApplyDrag();
        HandleMovement();
    }

    void HandleMovement()
    {
        if (enterExit == null || !enterExit.IsControlling) return;

        currentForwardInput = Input.GetAxis("Vertical");
        currentTurnInput = Input.GetAxis("Horizontal");

        // คำนวณ Rotation Y ใหม่ แล้วอัปเดต yRotation
        yRotation += currentTurnInput * turnSpeed * Time.fixedDeltaTime;

        // เคลื่อนที่
        Vector3 force = transform.forward * currentForwardInput * acceleration;
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void ApplyDrag()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 dragForce = -horizontalVelocity * waterDrag;
        rb.AddForce(dragForce, ForceMode.Acceleration);
    }
}
