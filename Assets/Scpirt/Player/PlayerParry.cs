using UnityEngine;
using System.Collections;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    public KeyCode parryKey = KeyCode.F; // ปุ่มสำหรับ Parry (สามารถเปลี่ยนได้)
    public float parryWindowDuration = 0.3f; // ระยะเวลาที่ Parry มีผล (หน่วยวินาที)
    public float parryCooldown = 1.0f; // Cooldown หลังจาก Parry (หน่วยวินาที)

    // *******************************************************************
    // การจัดการสถานะ Parry เพื่อให้ CharacterStats ตรวจสอบได้
    private bool _isCurrentlyParrying = false;
    public bool IsCurrentlyParrying // Property ที่ CharacterStats จะใช้ตรวจสอบ
    {
        get { return _isCurrentlyParrying; }
        private set { _isCurrentlyParrying = value; }
    }
    // *******************************************************************

    private bool _onParryCooldown = false;
    public bool OnParryCooldown
    {
        get { return _onParryCooldown; }
        private set { _onParryCooldown = value; }
    }

    [Header("Parry Success Effect")]
    public float parryDamageToBoss = 50f; // Boss จะโดนดาเมจเท่าไหร่เมื่อ Parry สำเร็จ
    public float bossStunDurationOnParry = 2.0f; // ระยะเวลาที่ Boss จะติด Stun เมื่อ Parry สำเร็จ

    [Header("References")]
    public CharacterStats characterStats;        // ลาก CharacterStats ของ Player มาใส่ใน Inspector
    public CharacterMovement characterMovement; // ลาก CharacterMovement ของ Player มาใส่ใน Inspector
    public Animator animator;                   // ลาก Animator ของ Player มาใส่ใน Inspector
    public BossAI bossAI;                       // ลาก Reference สำหรับ BossAI
    
    [Header("Parry Visual Feedback")] // <-- เพิ่ม Section นี้
    public SpriteRenderer parrySuccessIcon; // ลาก SpriteRenderer ของเครื่องหมายตกใจมาใส่ใน Inspector
    public float iconDisplayDuration = 0.5f; // ระยะเวลาที่ไอคอนจะแสดง

    [Header("Audio")]
    public AudioSource playerAudioSource; // ลาก AudioSource ของ Player มาใส่ใน Inspector
    public AudioClip parryAttemptSFX;     // เสียงเมื่อพยายาม Parry (กดปุ่ม)
    public AudioClip parrySuccessSFX;     // เสียงเมื่อ Parry สำเร็จ

    private Coroutine parryWindowRoutine;
    private Coroutine parryCooldownRoutine;
    private Coroutine iconDisplayRoutine; // <-- เพิ่ม Coroutine สำหรับไอคอน
    private BossStats bossStatsRef;

    void Awake()
    {
        // ตรวจสอบและดึง Reference อัตโนมัติ หากยังไม่ได้ลากใส่ใน Inspector
        if (characterStats == null) characterStats = GetComponent<CharacterStats>();
        if (characterMovement == null) characterMovement = GetComponent<CharacterMovement>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerAudioSource == null) playerAudioSource = GetComponent<AudioSource>();

        // ตรวจสอบว่า Reference ที่จำเป็นอยู่ครบหรือไม่
        if (characterStats == null) Debug.LogError("PlayerParry: CharacterStats component not found!", this);
        if (characterMovement == null) Debug.LogError("PlayerParry: CharacterMovement component not found!", this);
        
        // ค้นหา BossStats และ BossAI ในฉาก
        if (bossStatsRef == null) 
        {
            bossStatsRef = FindObjectOfType<BossStats>();
            if (bossStatsRef == null)
            {
                Debug.LogWarning("PlayerParry: BossStats component not found in the scene! Boss damage on parry will not work.", this);
            }
        }
        
        if (bossAI == null) 
        {
            bossAI = FindObjectOfType<BossAI>(); 
            if (bossAI == null)
            {
                Debug.LogWarning("PlayerParry: BossAI component not found in the scene! Boss stun on parry will not work.", this);
            }
        }
        
        // ตรวจสอบว่า parrySuccessIcon ถูกตั้งค่าหรือไม่
        // ถ้าไม่ได้ลากมาใน Inspector ให้พยายามหาจาก BossAI (ซึ่ง BossAI จะเป็น Parent ของ SpriteRenderer ที่อยู่บนหัวบอส)
        if (parrySuccessIcon == null && bossAI != null) 
        {
            parrySuccessIcon = bossAI.GetComponentInChildren<SpriteRenderer>();
            if (parrySuccessIcon != null && parrySuccessIcon.gameObject.name != "ParryFeedbackIcon") // ตรวจสอบชื่อเพื่อไม่ให้ดึง SpriteRenderer อื่นมา
            {
                // ถ้าหาเจอแต่ไม่ใช่ชื่อที่เราตั้งไว้ ให้เตือน
                Debug.LogWarning($"PlayerParry: Found a SpriteRenderer on BossAI children, but its name is '{parrySuccessIcon.gameObject.name}'. Did you mean to use 'ParryFeedbackIcon'?", this);
                parrySuccessIcon = null; // ตั้งค่าเป็น null เพื่อป้องกันการใช้ผิดตัว
            }
            // ถ้าเป็นชื่อที่เราตั้งไว้ ก็ใช้ตัวนั้นได้เลย
            // แต่ทางที่ดี ควรลากใส่ใน Inspector ตรงๆ จะชัวร์กว่า
        }

        if (parrySuccessIcon == null)
        {
            Debug.LogWarning("PlayerParry: Parry Success Icon (SpriteRenderer) reference not set! Visual feedback for parry success will not work.", this);
        }
        else
        {
            // ซ่อนไอคอนไว้ในตอนเริ่มต้น
            parrySuccessIcon.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(parryKey) && !IsCurrentlyParrying && !OnParryCooldown && !characterStats.isDead)
        {
            AttemptParry();
        }
    }

    void AttemptParry()
    {
        if (parryWindowRoutine != null) StopCoroutine(parryWindowRoutine);
        if (parryCooldownRoutine != null) StopCoroutine(parryCooldownRoutine);

        parryWindowRoutine = StartCoroutine(ParryWindowRoutine());
        parryCooldownRoutine = StartCoroutine(ParryCooldownRoutine());
    }

    IEnumerator ParryWindowRoutine()
    {
        IsCurrentlyParrying = true;
        Debug.Log("Player entered Parry Window!");

        if (characterMovement != null)
        {
            characterMovement.SetCanMove(false);
            Debug.Log("Player movement disabled for parry.");
        }

        if (animator != null)
        {
            animator.SetTrigger("Parry");
        }

        if (playerAudioSource != null && parryAttemptSFX != null)
        {
            playerAudioSource.PlayOneShot(parryAttemptSFX);
        }

        yield return new WaitForSeconds(parryWindowDuration);

        IsCurrentlyParrying = false;
        Debug.Log("Player exited Parry Window.");

        if (characterMovement != null && !OnParryCooldown)
        {
            characterMovement.SetCanMove(true);
            Debug.Log("Player movement enabled after parry window.");
        }
    }

    IEnumerator ParryCooldownRoutine()
    {
        OnParryCooldown = true;
        Debug.Log($"Parry on cooldown for {parryCooldown} seconds.");
        yield return new WaitForSeconds(parryCooldown);
        OnParryCooldown = false;
        Debug.Log("Parry cooldown finished.");

        if (characterMovement != null && !IsCurrentlyParrying)
        {
            characterMovement.SetCanMove(true);
            Debug.Log("Player movement enabled after parry cooldown.");
        }
    }

    public void OnParrySuccessful()
    {
        Debug.Log("PARRY SUCCESSFUL!");

        if (parryWindowRoutine != null)
        {
            StopCoroutine(parryWindowRoutine);
            parryWindowRoutine = null;
        }
        if (animator != null)
        {
            animator.ResetTrigger("Parry");
        }
        IsCurrentlyParrying = false;

        if (characterMovement != null)
        {
            characterMovement.SetCanMove(true);
            Debug.Log("Player movement enabled after successful parry.");
        }

        if (playerAudioSource != null && parrySuccessSFX != null)
        {
            playerAudioSource.PlayOneShot(parrySuccessSFX);
        }

        if (bossStatsRef != null)
        {
            bossStatsRef.TakeDamage(parryDamageToBoss);
            Debug.Log("Boss took " + parryDamageToBoss + " damage from parry.");
        }
        else
        {
            Debug.LogWarning("PlayerParry: BossStats reference is still null. Boss cannot take damage from parry.", this);
        }

        if (bossAI != null)
        {
            bossAI.StunBoss(bossStunDurationOnParry);
            Debug.Log($"Boss has been stunned for {bossStunDurationOnParry} seconds!");
        }
        else
        {
            Debug.LogWarning("PlayerParry: BossAI reference is missing. Boss cannot be stunned on parry.");
        }

        // *** แสดงไอคอนเครื่องหมายตกใจ! ***
        if (parrySuccessIcon != null)
        {
            if (iconDisplayRoutine != null)
            {
                StopCoroutine(iconDisplayRoutine); // หยุด Coroutine เก่าหากมีการ Parry ซ้ำเร็วๆ
            }
            iconDisplayRoutine = StartCoroutine(DisplayIconRoutine());
        }
    }

    IEnumerator DisplayIconRoutine() // <-- Coroutine ใหม่สำหรับแสดงไอคอน
    {
        parrySuccessIcon.enabled = true; // เปิดการแสดงผล Sprite
        Debug.Log("Parry Success Icon displayed.");

        yield return new WaitForSeconds(iconDisplayDuration); // รอตามระยะเวลาที่กำหนด

        parrySuccessIcon.enabled = false; // ปิดการแสดงผล Sprite
        Debug.Log("Parry Success Icon hidden.");
        iconDisplayRoutine = null; // ตั้งค่าเป็น null เมื่อ Coroutine จบ
    }

    public void ResetParryAnimatorTrigger()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Parry");
            Debug.Log("Animator Trigger 'Parry' has been reset by Animation Event.");
        }
    }
}