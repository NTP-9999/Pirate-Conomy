using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f); // ตำแหน่งห่างจากตัวละคร

    [Header("Rotation")]
    public float rotationSpeed = 5f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Zoom")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private float currentYaw = 0f;
    private float currentPitch = 10f;
    private float currentZoom;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("❌ ThirdPersonCamera: Target ไม่ถูกตั้งค่า!");
            enabled = false;
            return;
        }

        currentZoom = offset.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        // หมุนกล้องตามเมาส์
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // ซูมกล้องเข้าออก
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // คำนวณตำแหน่งกล้อง
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 direction = rotation * Vector3.back * currentZoom;
        Vector3 cameraPosition = target.position + direction + Vector3.up * offset.y;

        transform.position = cameraPosition;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }
}
