using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    PlayerController pc;

    void Awake() {
        pc = GetComponent<PlayerController>();
    }

    void Update() {
        // ตัวอย่าง: ปุ่ม Q ใช้สกิล “Fireball”
        if (Input.GetKeyDown(KeyCode.F) && SkillManager.Instance.IsUnlocked("FireWall")) {
            CastFireWall();
        }
    }

    void CastFireWall() {
        Debug.Log("Cast FireWall!");    
        // สร้าง projectile, สร้าง VFX ฯลฯ
    }
}
