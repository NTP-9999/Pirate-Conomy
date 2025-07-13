using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class CharacterStats : Singleton<CharacterStats>
{
    [Header("Health")]
    [SerializeField] public float _currentHealth;
    public float maxHealth = 100f;

    public float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (_currentHealth < 0) _currentHealth = 0;
            // FIX: Corrected typo from _currentH_currentHealth to _currentHealth
            if (_currentHealth > maxHealth) _currentHealth = maxHealth;
        }
    }


    [Header("Stamina")]
    [SerializeField] private float _currentStamina;
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1.0f;
    
    public float currentStamina
    {
        get { return _currentStamina; }
        set
        {
            _currentStamina = value;
            if (_currentStamina < 0) _currentStamina = 0;
            if (_currentStamina > maxStamina) _currentStamina = maxStamina;
        }
    }

    private Coroutine staminaRegenCoroutine;
    public bool isRunningStaminaDrain { get; set; } = false;
    private float lastStaminaUseTime;
    [Header("Hunger")]
    public float currentHunger = 100f;
    public float maxHunger = 100f;
    public float hungerDecreaseIntervalMinutes = 3f;

    [Header("Stress")]
    public float currentStress = 0f;
    public float maxStress = 100f;
    public float stressIncreaseIntervalMinutes = 5f;

    public bool isDead { get; private set; } = false;
    private AudioSource audioSource;
    private CharacterController characterController;
    private Animator animator;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip damageSFX;

    public event Action<float> OnPlayerDamaged;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
        currentStress = 0f;
        isDead = false;

        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component for death sound not found on " + gameObject.name + ". Death sound will not play.");
        }

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name + ". Player animations (Hurt/Death) will not play.");
        }

        if (playerAudioSource == null)
        {
            playerAudioSource = audioSource;
            if (playerAudioSource == null)
            {
                Debug.LogWarning("Player AudioSource for damage/heal SFX not found. Damage/Heal SFX might not play.");
            }
        }

        StartCoroutine(HungerDecreaseRoutine());
        StartCoroutine(StressIncreaseRoutine());
    }

    private void Update()
    {
        if (isDead) return;

        if (isRunningStaminaDrain && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            lastStaminaUseTime = Time.time;

            if (staminaRegenCoroutine != null)
            {
                StopCoroutine(staminaRegenCoroutine);
                staminaRegenCoroutine = null;
            }
        }

        if (!isRunningStaminaDrain && currentStamina < maxStamina && Time.time >= lastStaminaUseTime + staminaRegenDelay)
        {
            if (staminaRegenCoroutine == null)
            {
                staminaRegenCoroutine = StartCoroutine(RegenStaminaRoutine());
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Current Health: " + currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
            Debug.Log("Player Animator: Set 'Hurt' Trigger.");
        }

        if (playerAudioSource != null && damageSFX != null)
        {
            playerAudioSource.PlayOneShot(damageSFX);
            Debug.Log("Played Damage SFX.");
        }
        else
        {
            if (playerAudioSource == null) Debug.LogWarning("Damage SFX: playerAudioSource is null.");
            if (damageSFX == null) Debug.LogWarning("Damage SFX: damageSFX AudioClip is null.");
        }
        PlayerStateMachine stateMachine = GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(stateMachine.hurtState);
        }

        OnPlayerDamaged?.Invoke(amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void StartStaminaDrain()
    {
        if (isDead) return;
        isRunningStaminaDrain = true;
        lastStaminaUseTime = Time.time;
        if (staminaRegenCoroutine != null)
        {
            StopCoroutine(staminaRegenCoroutine);
            staminaRegenCoroutine = null;
        }
    }

    public void StopStaminaDrain()
    {
        if (isDead) return;
        isRunningStaminaDrain = false;
    }

    public void UseStamina(float amount)
    {
        if (isDead) return;

        currentStamina -= amount;
        lastStaminaUseTime = Time.time;

        if (staminaRegenCoroutine != null)
        {
            StopCoroutine(staminaRegenCoroutine);
            staminaRegenCoroutine = null;
        }
    }

    private IEnumerator RegenStaminaRoutine()
    {
        while (!isDead && !isRunningStaminaDrain && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            yield return null;
        }
        staminaRegenCoroutine = null;
    }
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

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player has died!");

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("Death");
            Debug.Log("Player Animator: Set 'IsDead' Bool and 'Death' Trigger.");
        }

        CharacterMovement movementScript = GetComponent<CharacterMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        FirstPersonCamera fpCamera = GetComponentInChildren<FirstPersonCamera>();
        if (fpCamera != null)
        {
            fpCamera.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        StopAllCoroutines();
        Destroy(gameObject , 3f);
    }

    void OnDeathAnimationEnd()
    {
        Debug.Log("Game Over! Character has finished death animation.");
    }
}