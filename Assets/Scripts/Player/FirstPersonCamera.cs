using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f; 
    public float verticalLookLimit = 80f; 

    private Transform playerBody; 
    private float xRotation = 0f; 

    // เก็บตำแหน่งเริ่มต้นของกล้องเทียบกับ FPS_CameraHolder
    private Vector3 initialLocalCameraPosition; 

    // อ้างอิง CharacterStats (สำหรับฟังก์ชันการตาย)
    private CharacterStats characterStats; 
    
    // *** สำคัญ: ไม่มีตัวแปรหรือ Headers สำหรับ Camera Shake ในส่วนนี้แล้ว ***
    // เช่น ไม่มี public float runShakeIntensity;
    // ไม่มี private Vector3 currentShakeOffset;
    // ไม่มี public bool IsRunning;

    void Start()
    {
        // ตรวจสอบว่าสคริปต์นี้อยู่บน Main Camera หรือ FPS_CameraHolder ที่มี Parent เป็น Player
        initialLocalCameraPosition = transform.localPosition; 
        playerBody = transform.parent; 
        if (playerBody == null)
        {
            Debug.LogError("Player Body (parent GameObject) not found for FirstPersonCamera script. Please ensure this script is on a GameObject that is a child of your Player.");
        }

        // ล็อคเคอร์เซอร์เมาส์และซ่อนมัน
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 

        // ดึง CharacterStats มาจาก Parent ของกล้อง (ซึ่งน่าจะเป็นตัวละครหลัก)
        characterStats = GetComponentInParent<CharacterStats>(); 
        if (characterStats == null)
        {
            Debug.LogWarning("CharacterStats component not found on parent of camera. Camera control might not stop on death.");
        }

        // กำหนดการหมุนเริ่มต้นของกล้องตามค่าปัจจุบัน
        xRotation = transform.localEulerAngles.x;
    }

    void Update()
    {
        // ตรวจสอบสถานะการตาย
        if (characterStats != null && characterStats.isDead)
        {
            this.enabled = false; // ปิดสคริปต์ FirstPersonCamera
            Cursor.lockState = CursorLockMode.None; // ปลดล็อคเมาส์
            Cursor.visible = true; // ทำให้เมาส์มองเห็นได้
            Debug.Log("FirstPersonCamera: Player is dead, disabling camera control.");
            return; // หยุดการทำงานของ Update ทันที
        }

        // Input เมาส์
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // การหมุนแกน X (ขึ้น-ลง) สำหรับกล้อง
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        // การหมุนแกน Y (ซ้าย-ขวา) สำหรับตัวละคร (playerBody)
        playerBody.Rotate(Vector3.up * mouseX);

        // ESC เพื่อปลดล็อค/ล็อคเคอร์เซอร์
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // ใช้ LateUpdate เพื่อให้แน่ใจว่าการเคลื่อนไหวและการหมุนของกล้องเสร็จสิ้นหลังการอัปเดตของตัวละคร
    void LateUpdate()
    {
        // ตรวจสอบสถานะการตายอีกครั้ง (เพื่อความปลอดภัย)
        if (characterStats != null && characterStats.isDead)
        {
            return; 
        }

        // กำหนดการหมุนของกล้อง (เฉพาะแกน X)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // กำหนดตำแหน่งกล้องให้เป็นตำแหน่งเริ่มต้นเสมอ (ไม่มีการสั่นใดๆ เพิ่มเติม)
        transform.localPosition = initialLocalCameraPosition; 
    }
}