using UnityEngine;
using UnityEngine.UI;       // ต้องมี
// using TMPro;             // ถ้าไม่ใช้ตัวเลขก็ไม่ต้องใส่

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Bar Images (Fill)")]
    [SerializeField] private Image HP_Fill;
    [SerializeField] private Image STA_Fill;

    private CharacterStats characterStats;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            characterStats = player.GetComponent<CharacterStats>();
        else
            Debug.LogError("Player not found! ติด Tag 'Player' ให้ถูกต้อง");
    }

    void Update()
    {
        if (characterStats == null) return;

        // ถ้า CharacterStats ยังไม่มี maxHealth / maxStamina ให้เพิ่มเข้าไปด้วยนะ
        HP_Fill.fillAmount = characterStats.currentHealth 
                           / characterStats.maxHealth;
        STA_Fill.fillAmount = characterStats.currentStamina 
                            / characterStats.maxStamina;
    }
}
