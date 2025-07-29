using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class ShipStats : MonoBehaviour
{
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI")]
    [SerializeField] private Image HP_Fill;

    [Header("Game Over")]
    public GameObject diePanel;

    private bool isSinking = false;

    [Header("Audio & VFX")]
    [Tooltip("ตัวเล่นเสียงบนเรือ (AudioSource)")]
    public AudioSource audioSource;
    [Tooltip("เสียงตอนโดนตี")]
    public AudioClip hitSound;
    [Tooltip("เสียงตอนเรือจม")]
    public AudioClip deathSound;
    [Tooltip("Prefab ของ ParticleSystem ตอนโดนตี")]
    public GameObject hitVfxPrefab;
    [Tooltip("Prefab ของ ParticleSystem ตอนเรือจม")]
    public GameObject deathVfxPrefab;

    [Header("Sinking Animation")]
    [Tooltip("ระยะเวลาที่ใช้จม (วินาที)")]
    public float sinkDuration = 3f;
    [Tooltip("ความลึกที่จะจมลง (หน่วยยูนิตี้)")]
    public float sinkDepth = 5f;
    [Tooltip("มุมที่จะพลิกเรือ (Pitch)")]
    public float sinkPitchAngle = 20f;
    private GameObject player;
    

    void Start()
    {
        player = GameObject.FindWithTag("Player");
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
        if (hitSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(hitSound);
            else
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // Spawn hit VFX
        if (hitVfxPrefab != null)
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);

        currentHealth -= amount;
        Debug.Log($"🛳️ เรือได้รับความเสียหาย {amount} หน่วย! เหลือ: {currentHealth}");
        UpdateUI();
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
        StartCoroutine(SinkAndKillPlayer());
        isSinking = true;
        Debug.Log("💥 เรือจมแล้ว!");

        // แสดง Game Over UI
        if (diePanel != null)
            diePanel.SetActive(true);

        // ปิดการควบคุมเรือ
        var controller = GetComponent<ShipController>();
        if (controller != null)
            controller.enabled = false;

        // เล่นเสียงจม
        if (deathSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(deathSound);
            else
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // สร้าง VFX จม
        if (deathVfxPrefab != null)
            Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);

        // เริ่มอนิเมชั่นจม
        StartCoroutine(SinkAnimation());
    }
    private IEnumerator SinkAndKillPlayer()
    {
        // 1) รอให้เรือจมจนจบ
        yield return StartCoroutine(SinkAnimation());

        // 2) ทำลายผู้เล่น
        if (player != null)
            Destroy(player);

        // 3) สุดท้ายทำลายเรือ (ถ้ายังไม่ถูกทำลาย)
        Destroy(gameObject);
    }

    private IEnumerator SinkAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * sinkDepth;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(sinkPitchAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / sinkDuration);

            // Lerp position + Slerp rotation
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        // จบแล้วก็ล็อกค่าที่สุดท้ายไว้
        transform.position = endPos;
        transform.rotation = endRot;
    }
}
