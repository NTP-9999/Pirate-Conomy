using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;            // ตัวละคร
    public Vector3 offset = new Vector3(0, 2, -4); // ตำแหน่งกล้องห่างจากตัวละคร

    [Header("Rotation")]
    public float sensitivity = 2f;
    public float minY = -40f;
    public float maxY = 80f;

    [Header("Zoom")]
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    private float currentX = 0f;
    private float currentY = 10f;
    private float distance;
    public float Yaw => currentX;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("❌ ThirdPersonCamera: ไม่ได้ตั้ง Target (ตัวละคร)");
            enabled = false;
            return;
        }

        distance = offset.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // หมุนกล้องตามเมาส์
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY -= Input.GetAxis("Mouse Y") * sensitivity;
        currentY = Mathf.Clamp(currentY, minY, maxY);

        // ซูมเข้าออกด้วย Mouse Scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // คำนวณตำแหน่งใหม่ของกล้อง
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back * distance;

        transform.position = target.position + direction + Vector3.up * offset.y;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}
