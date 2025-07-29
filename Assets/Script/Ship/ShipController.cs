using UnityEngine;

public class ShipController : Singleton<ShipController>
{
    public static ShipController Instance;
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float turnSpeed = 30f;
    public float waterDrag = 0.5f;

    public float rockingAmplitude = 2f; // องศาการโยก
    public float rockingFrequency = 0.5f; // ความถี่
    private float rockingTimer = 0f;
    private bool cheatMode = false;
    private float defaultAcceleration;
    private float defaultMaxSpeed;
    private const float cheatAcceleration = 9999999999999f;
    private const float cheatMaxSpeed     = 200f;
    public Vector3 shipPosition
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Transform shipVisual; // drag ShipVisual (child) มาใส่ใน Inspector

    private Rigidbody rb;
    private ShipEnterExit enterExit;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // มีตัวแรกอยู่แล้ว → ฆ่าตัวนี้ทิ้ง
            Destroy(gameObject);
            return;
        }

        // ไม่เคยมีตัวไหนเลย → บันทึกตัวนี้ไว้, และไม่ทำลายข้ามซีน
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        enterExit = GetComponent<ShipEnterExit>();
        rb = GetComponent<Rigidbody>();
        defaultAcceleration = acceleration;
        defaultMaxSpeed     = maxSpeed;
    }
    void Update()
    {
        // ─── 2) Toggle ปุ่ม Y ─────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cheatMode = !cheatMode;
            if (cheatMode)
            {
                acceleration = cheatAcceleration;
                maxSpeed     = cheatMaxSpeed;
                Debug.Log("<color=green>Cheat ON:</color> Accel=100, MaxSpeed=100");
            }
            else
            {
                acceleration = defaultAcceleration;
                maxSpeed     = defaultMaxSpeed;
                Debug.Log("<color=yellow>Cheat OFF:</color> Restored defaults");
            }
        }
    }

    void FixedUpdate()
    {
        ApplyDrag();
        HandleMovement();

        // Rocking (โยก Pitch/Roll ที่ mesh เท่านั้น)
        rockingTimer += Time.fixedDeltaTime;
        float pitch = Mathf.Sin(rockingTimer * rockingFrequency * Mathf.PI * 2f) * rockingAmplitude;
        shipVisual.localRotation = Quaternion.Euler(pitch, 0, 0);

        // ล็อก Rotation X/Z ของ Rigidbody หลัก
        Vector3 euler = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0, euler.y, 0);
    }

    void HandleMovement()
    {
        if (enterExit == null || !enterExit.isControlling) return;

        float forward = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        // เคลื่อนที่
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(transform.forward * forward * acceleration, ForceMode.Force);

        // เลี้ยว
        rb.AddTorque(Vector3.up * turn * turnSpeed, ForceMode.Force);
    }

    void ApplyDrag()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 dragForce = -horizontalVelocity * waterDrag;
        rb.AddForce(dragForce, ForceMode.Force);
    }
}
