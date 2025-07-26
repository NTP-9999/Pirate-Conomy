using UnityEngine;
using UnityEngine.Playables;

public class ShipStats : MonoBehaviour
{
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("Game Over")]
    public GameObject diePanel;

    [Header("Timeline")]
    public PlayableDirector deathTimeline;

    private bool isSinking = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // ป้องกันเรียกซ้ำ
        if (!isSinking && currentHealth <= 0)
        {
            SinkShip();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"🛳️ เรือได้รับความเสียหาย {amount} หน่วย! เหลือ: {currentHealth}");
    }

    void SinkShip()
    {
        isSinking = true;
        Debug.Log("💥 เรือจมแล้ว!");

        // 1. เล่น Timeline
        if (deathTimeline != null)
        {
            deathTimeline.Play();
        }

        // 2. กล้องตาย
        var deathCam = Camera.main.GetComponent<DeathCameraTransition>();
        if (deathCam != null)
            deathCam.StartTransition();

        // 3. แสดง Game Over UI
        if (diePanel != null)
            diePanel.SetActive(true);

        // 4. ปิดระบบควบคุมเรือ
        var controller = GetComponent<ShipController>();
        if (controller != null)
            controller.enabled = false;
    }
}
