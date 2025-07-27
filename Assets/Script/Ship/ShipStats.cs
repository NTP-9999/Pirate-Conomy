using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ShipStats : MonoBehaviour
{
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI")]
    [SerializeField] private Image HP_Fill;

    [Header("Game Over")]
    public GameObject diePanel;

    [Header("Timeline")]
    public PlayableDirector deathTimeline;

    private bool isSinking = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI(); // อัปเดตค่าเริ่มต้น
    }

    void Update()
    {
        if (!isSinking && currentHealth <= 0)
        {
            SinkShip();
        }
    }

    public float GetHealthPercent()
    {
        return Mathf.Clamp01(currentHealth / maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"🛳️ เรือได้รับความเสียหาย {amount} หน่วย! เหลือ: {currentHealth}");

        UpdateUI(); // อัปเดต UI ทุกครั้งที่โดนดาเมจ
    }

    void UpdateUI()
    {
        if (HP_Fill != null)
        {
            HP_Fill.fillAmount = GetHealthPercent();
        }
    }

    void SinkShip()
    {
        isSinking = true;
        Debug.Log("💥 เรือจมแล้ว!");

        if (deathTimeline != null)
            deathTimeline.Play();

        if (diePanel != null)
            diePanel.SetActive(true);

        var controller = GetComponent<ShipController>();
        if (controller != null)
            controller.enabled = false;
    }
}
