using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("Bindings")]
    public string skillName;           // ชื่อสกิลให้ตรงกับ SkillManager.skillName
    public Image icon;
    public GameObject shackleOverlay;  // assign GameObject ของโซ่ตรวนใน Inspector

    void Start() {
        // ตอนเริ่ม ให้ซ่อนโซ่ตรวนถ้าสตेटสกิลปลดล็อกแล้ว
        shackleOverlay.SetActive(!SkillManager.Instance.IsUnlocked(skillName));
        // ลงทะเบียนฟังอีเวนต์
        SkillManager.Instance.OnSkillUnlocked += HandleSkillUnlocked;
    }

    void OnDestroy() {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillUnlocked -= HandleSkillUnlocked;
    }

    private void HandleSkillUnlocked(string unlockedSkill) {
        if (unlockedSkill == skillName) {
            // ปลดล็อกสกิลตัวนี้ → ซ่อนโซ่ตรวน
            shackleOverlay.SetActive(false);
            // (ถ้าต้องการ effect อะไรเพิ่มเติมก็ใส่ตรงนี้ได้)
        }
    }

    /// <summary>
    /// เรียกตอนอัพเดตไอคอน หรือรีเซ็ต UI
    /// </summary>
    public void SetIcon(Sprite newIcon) {
        icon.sprite = newIcon;
    }
}
