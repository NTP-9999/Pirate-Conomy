using UnityEngine;
using System.Collections; // จำเป็นสำหรับการใช้ Coroutine

public class CharacterStats : MonoBehaviour
{
    // ค่าสถานะ
    [Header("Health")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    [Header("Stamina")]
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    [Tooltip("อัตราการใช้ Stamina ต่อวินาทีเมื่อวิ่ง")]
    public float staminaDrainRate = 10f; 
    [Tooltip("อัตราการฟื้นฟู Stamina ต่อวินาทีเมื่อไม่วิ่ง")]
    public float staminaRegenRate = 15f; 
    [Tooltip("ระยะเวลาที่ต้องรอหลังจากหยุดวิ่งก่อน Stamina จะเริ่มฟื้นฟู")]
    public float staminaRegenDelay = 1.0f; 

    private Coroutine staminaRegenCoroutine; 
    private bool isRunningStaminaDrain = false; 

    [Header("Hunger")]
    public float currentHunger = 100f;
    public float maxHunger = 100f;
    [Tooltip("เวลา (นาที) ที่ Hunger จะลดลง 1 หน่วย")]
    public float hungerDecreaseIntervalMinutes = 3f; 

    [Header("Stress")]
    public float currentStress = 0f;
    public float maxStress = 100f; 
    [Tooltip("เวลา (นาที) ที่ Stress จะเพิ่มขึ้น 1 หน่วย")]
    public float stressIncreaseIntervalMinutes = 5f; 

    public bool isDead { get; private set; } = false; 
    private FadeEffect fadeEffect; // ตัวแปรสำหรับ FadeEffect ถ้ามี

    // สำคัญมาก: ประกาศตัวแปรสำหรับ Components อื่นๆ
    private AudioSource audioSource; 
    private CharacterController characterController; 
    private Animator animator; 

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
        currentStress = 0f;
        isDead = false;
        fadeEffect = FindObjectOfType<FadeEffect>(); // ค้นหา FadeEffect ใน Scene ถ้ามี
        if (fadeEffect == null)
        {
            Debug.LogError("FadeEffect component not found in the scene! Make sure the UIManager/GameManager GameObject with FadeEffect script exists.");
        }
        // สำคัญมาก: ดึง Component ต่างๆ มาใช้งานใน Start()
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on " + gameObject.name + ". Death sound will not play.");
        }

        characterController = GetComponent<CharacterController>();
        // ไม่ต้อง Error ถ้าไม่มี CharacterController เพราะบางตัวละครอาจไม่ได้ใช้

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name + ". Death animation will not play.");
        }

        StartCoroutine(HungerDecreaseRoutine());
        StartCoroutine(StressIncreaseRoutine());
    }

    private void Update()
    {
        // Debug.Log($"Health: {currentHealth}, Stamina: {currentStamina}, Hunger: {currentHunger}, Stress: {currentStress}");
    }

    // --- Health Methods ---
    public void TakeDamage(float amount)
    {
        if (isDead) return; 

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // --- Stamina Methods ---
    public void StartStaminaDrain()
    {
        if (isDead) return;

        if (staminaRegenCoroutine != null)
        {
            StopCoroutine(staminaRegenCoroutine); 
        }
        isRunningStaminaDrain = true;
        StartCoroutine(DrainStaminaRoutine());
    }

    public void StopStaminaDrain()
    {
        if (isDead) return;

        isRunningStaminaDrain = false;
        if (staminaRegenCoroutine != null)
        {
             StopCoroutine(staminaRegenCoroutine);
        }
        staminaRegenCoroutine = StartCoroutine(RegenStaminaRoutine());
    }

    private IEnumerator DrainStaminaRoutine()
    {
        while (isRunningStaminaDrain && currentStamina > 0 && !isDead)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
            yield return null; 
        }
        isRunningStaminaDrain = false; 
        if (!isDead) 
        {
            StopStaminaDrain(); 
        }
    }

    private IEnumerator RegenStaminaRoutine()
    {
        yield return new WaitForSeconds(staminaRegenDelay); 
        while (!isRunningStaminaDrain && currentStamina < maxStamina && !isDead)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            yield return null; 
        }
    }

    // --- Hunger Methods ---
    private IEnumerator HungerDecreaseRoutine()
    {
        float intervalSeconds = hungerDecreaseIntervalMinutes * 60f;
        while (!isDead) 
        {
            yield return new WaitForSeconds(intervalSeconds);
            if (isDead) yield break; 

            currentHunger -= 1;
            if (currentHunger <= 0) 
            {
                currentHunger = 0;
                Debug.Log("Hunger: " + currentHunger + " - Character is starving!");
                Die(); 
            }
            else
            {
                Debug.Log("Hunger: " + currentHunger);
            }
        }
    }

    public void Eat(float amount)
    {
        if (isDead) return;

        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
    }

    // --- Stress Methods ---
    private IEnumerator StressIncreaseRoutine()
    {
        float intervalSeconds = stressIncreaseIntervalMinutes * 60f;
        while (!isDead) 
        {
            yield return new WaitForSeconds(intervalSeconds);
            if (isDead) yield break; 

            currentStress += 1;
            if (currentStress >= maxStress) 
            {
                currentStress = maxStress;
                Debug.Log("Stress: " + currentStress + " - Character is overwhelmed!");
                Die(); 
            }
            else
            {
                Debug.Log("Stress: " + currentStress);
            }
        }
    }

    public void DecreaseStress(float amount)
    {
        if (isDead) return;
        currentStress -= amount;
        if (currentStress < 0)
        {
            currentStress = 0;
        }
    }

    // --- Other Methods ---
    void Die()
    {
        if (isDead) return; // ป้องกันการเรียก Die() ซ้ำซ้อน

        isDead = true; 
        Debug.Log("Character has died!");

        // 1. เล่นเสียงตาย
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // 2. เล่นแอนิเมชันตาย
        if (animator != null)
        {
            animator.SetBool("IsDead", true); 
        }

        // 3. หยุดการเคลื่อนไหวและการชนของตัวละคร
        CharacterMovement movementScript = GetComponent<CharacterMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false; // ปิด Script การเคลื่อนไหว
        }
        if (characterController != null) 
        {
            characterController.enabled = false; // ปิด CharacterController เพื่อหยุดการชน
        }
        
         if (fadeEffect != null)
        {
            fadeEffect.StartFadeToBlack();
        }
        else
        {
            Debug.LogError("FadeEffect is null, cannot start fade to black.");
        }
        
        // ถ้าตัวละครมี Rigidbody และคุณต้องการให้มันล้มลงตามหลักฟิสิกส์
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // ทำให้ Rigidbody ไม่ใช่ Kinematic (ควบคุมด้วยฟิสิกส์)
            rb.useGravity = true;   // เปิดใช้แรงโน้มถ่วง
            rb.linearVelocity = Vector3.zero; // หยุดการเคลื่อนที่ปัจจุบัน
            rb.angularVelocity = Vector3.zero; // <--- สำคัญ: หยุดการหมุนปัจจุบัน!
            // อาจจะเพิ่มแรงผลักเล็กน้อยก็ได้: rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        }

        // 4. หยุด Coroutine ทั้งหมดที่เกี่ยวกับค่าสถานะ (Stamina, Hunger, Stress)
        StopAllCoroutines(); 

        // 5. ตัวเลือก: จัดการหลังแอนิเมชันตายจบ (เช่น โหลดฉากใหม่, แสดงหน้าจอ Game Over)
        float deathAnimationLength = 3f; // ค่าเริ่มต้น
        if (animator != null)
        {
            // ลองหาความยาวของคลิปแอนิเมชัน "Death" โดยตรง
            // วิธีนี้อาจจะยังไม่แม่นยำ 100% ถ้า Animator Controller มีซับซ้อนมาก
            // แต่ก็เป็นวิธีที่ค่อนข้างใช้ได้
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Contains("Death") || clip.name.Contains("die") || clip.name.Contains("Die")) 
                {
                    deathAnimationLength = clip.length;
                    break;
                }
            }
        } else if (audioSource != null && audioSource.clip != null) {
            deathAnimationLength = audioSource.clip.length;
        }

        // เรียกฟังก์ชันสำหรับ Game Over หลังจากหน่วงเวลา
        Invoke("OnDeathAnimationEnd", deathAnimationLength + 1f); 
    }

    // ฟังก์ชันนี้จะถูกเรียกหลังจากแอนิเมชันตายจบ (ตาม Invoke)
    void OnDeathAnimationEnd()
    {
        Debug.Log("Game Over! Character has finished death animation.");
    }
}