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

    // <--- เพิ่มตัวแปรสำหรับอ้างอิง CharacterStats
    private CharacterStats characterStats; 

    void Start()
    {
        initialLocalCameraPosition = transform.localPosition; 
        playerBody = transform.parent; 
        if (playerBody == null)
        {
            Debug.LogError("Player Body (parent GameObject) not found for FirstPersonCamera script.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // ทำให้เมาส์หายไปตอนเริ่มเกม

        // Set initial rotation
        xRotation = transform.localEulerAngles.x;

        // *** เก็บตำแหน่ง localPosition เริ่มต้นของ Main Camera (0,0,0) ***
        // บรรทัดนี้ซ้ำกับบรรทัดแรกใน Start() สามารถลบบรรทัดแรกทิ้งได้
        // initialLocalCameraPosition = transform.localPosition; 

        // <--- ดึง CharacterStats มาจาก Parent ของกล้อง (ซึ่งน่าจะเป็นตัวละคร)
        characterStats = GetComponentInParent<CharacterStats>(); 
        if (characterStats == null)
        {
            Debug.LogError("CharacterStats component not found on parent of camera. Camera control might not stop on death. Please ensure CharacterStats is on the Player GameObject.");
        }
    }

    void Update()
    {
        // <--- สำคัญ: ตรวจสอบสถานะการตายก่อนประมวลผล Input
        if (characterStats != null && characterStats.isDead)
        {
            // ถ้าตัวละครตายแล้ว ให้ปิดสคริปต์นี้
            this.enabled = false; 
            // ปลดล็อคเมาส์และทำให้มองเห็น
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("FirstPersonCamera: Player is dead, disabling camera control.");
            return; // หยุดการทำงานของ Update ทันที
        }

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // X axis rotation (up-down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        // Y axis rotation (left-right) for player body
        playerBody.Rotate(Vector3.up * mouseX);

        // ESC to unlock cursor (ยังคงมีประโยชน์สำหรับการหยุดเกมชั่วคราว)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // สลับสถานะล็อคเคอร์เซอร์
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

    // *** ใช้ LateUpdate เพื่อปรับตำแหน่งกล้องหลังแอนิเมชันทำงานแล้ว ***
    void LateUpdate()
    {
        // <--- สำคัญ: ตรวจสอบสถานะการตายก่อนประมวลผล Input (อีกครั้งสำหรับ LateUpdate)
        // เพื่อป้องกันการเคลื่อนไหวที่ไม่ต้องการหากสคริปต์ถูกปิดใน Update แต่ LateUpdate ยังทำงานในเฟรมเดียวกัน
        if (characterStats != null && characterStats.isDead)
        {
            return; // หยุดการทำงานของ LateUpdate ทันที
        }

        // กำหนดการหมุนของกล้อง (X-axis)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.localPosition = initialLocalCameraPosition; 
    }
}