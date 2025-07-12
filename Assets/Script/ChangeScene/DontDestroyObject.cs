using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    void Awake()
    {
        // หา GameObject ทั้งหมดที่มีสคริปต์นี้
        DontDestroyObject[] existingObjects = FindObjectsOfType<DontDestroyObject>();
        
        // ตรวจสอบว่ามี GameObject เดียวกัน (ชื่อเดียวกัน) อยู่แล้วหรือไม่
        foreach (var obj in existingObjects)
        {
            if (obj != this && obj.gameObject.name == gameObject.name)
            {
                Debug.Log($"Duplicate {gameObject.name} found, destroying: {gameObject.name}");
                Destroy(gameObject);
                return;
            }
        }
        
        // ถ้าไม่มีตัวซ้ำ ให้ใช้ DontDestroyOnLoad
        Object.DontDestroyOnLoad(gameObject);  // เปลี่ยนเป็น Object.DontDestroyOnLoad
    }
}
