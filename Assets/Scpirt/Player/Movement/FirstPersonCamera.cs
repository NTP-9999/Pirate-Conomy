using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("Camera Constraints")]
    public float maxLookAngle = 80f;
    public float minLookAngle = -80f;

    [Header("Optional Player Body Reference")]
    public Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;

    // *** เพิ่มสำหรับควบคุมการยกกล้อง ***
    private bool isElevatingCamera = false;
    private float targetElevatedLocalY = 0f; // ตำแหน่ง Y ปลายทางของกล้อง (local position)
    public float elevationSpeed = 1f; // ความเร็วในการยกกล้อง

    private float originalLocalYPosition; // เก็บค่า localPosition.y เริ่มต้นของกล้อง

    // *** STATIC FLAG: ควบคุมว่ากล้องถูกยกขึ้นแล้วหรือไม่ (ทั่วทั้งเกม) ***
    private static bool hasCameraElevatedGlobally = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerBody == null && transform.parent != null)
        {
            playerBody = transform.parent;
        }

        originalLocalYPosition = transform.localPosition.y;
    }

    void Update()
    {
        HandleMouseLook();
        HandleInput();

        if (isElevatingCamera)
        {
            Vector3 currentLocalPos = transform.localPosition;
            currentLocalPos.y = Mathf.MoveTowards(currentLocalPos.y, targetElevatedLocalY, elevationSpeed * Time.deltaTime);
            transform.localPosition = currentLocalPos;

            if (Mathf.Abs(transform.localPosition.y - targetElevatedLocalY) < 0.001f)
            {
                isElevatingCamera = false;
                Debug.Log("Camera reached target Y position.");
                // ไม่ต้องทำอะไรเพิ่มที่นี่ เพราะ hasCameraElevatedGlobally จะถูกตั้งใน StartCameraElevation
            }
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SetCameraRotation(float x, float y)
    {
        xRotation = Mathf.Clamp(x, minLookAngle, maxLookAngle);
        yRotation = y;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    public Vector2 GetCameraRotation()
    {
        return new Vector2(xRotation, yRotation);
    }

    // *** เมธอดสาธารณะสำหรับสั่งให้กล้องยกตัวขึ้น ***
    public void StartCameraElevation(float targetYOffset, float speed)
    {
        // *** ตรวจสอบ Flag ทั่วโลกก่อนที่จะเริ่มการยกกล้อง ***
        if (hasCameraElevatedGlobally)
        {
            Debug.LogWarning("Camera has already been elevated globally. Skipping elevation.");
            return; // ไม่ต้องทำอะไรแล้ว
        }

        targetElevatedLocalY = originalLocalYPosition + targetYOffset;
        elevationSpeed = speed;
        isElevatingCamera = true;
        hasCameraElevatedGlobally = true; // ตั้งค่า Flag ทั่วโลกเมื่อเริ่มการยกกล้องครั้งแรก
        Debug.Log($"Camera starting global elevation to local Y: {targetElevatedLocalY} at speed {speed}");
    }

    // *** เมธอดสาธารณะสำหรับสั่งให้กล้องกลับตำแหน่งเดิม ***
    // เมธอดนี้จะยังคงทำงานได้เสมอ ไม่ได้ถูกจำกัดด้วย hasCameraElevatedGlobally
    public void ResetCameraElevation(float speed)
    {
        // ควรเรียกเมธอดนี้เมื่อ Dialogue/Cutscene จบลง
        // ถ้าต้องการให้กล้องกลับมาสู่ตำแหน่งเดิม (แม้ว่าจะถูกยกขึ้นเพียงครั้งเดียวก็ตาม)
        targetElevatedLocalY = originalLocalYPosition;
        elevationSpeed = speed * 12f;
        isElevatingCamera = true;
        Debug.Log($"Camera resetting elevation to local Y: {originalLocalYPosition} at speed {elevationSpeed}");
    }

    // *** อาจเพิ่มเมธอดสำหรับรีเซ็ต hasCameraElevatedGlobally ในกรณีที่ต้องการทดสอบหรือรีสตาร์ทเกม ***
    // แต่สำหรับการใช้งานจริงในเกม ควรจะไม่มีการเรียกเมธอดนี้
    public static void ResetGlobalCameraElevationFlag()
    {
        hasCameraElevatedGlobally = false;
        Debug.Log("Global camera elevation flag has been reset (for testing/debugging).");
    }
}