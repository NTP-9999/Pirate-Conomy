using UnityEngine;

public class PlayerAudioManager : Singleton<PlayerAudioManager>
{
    [Header("SFX Clips")]
    public AudioClip footstepClip;
    public AudioClip runClip;
    public AudioClip rollClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip attack1Clip;
    public AudioClip attack2Clip;
    public AudioClip attack3Clip;
    public AudioClip blockClip;
    public AudioClip parryClip;
    public AudioClip punchClip;
    public AudioClip firewall;
    public AudioClip firewalldot;
    public AudioClip opencloseShopSound;
    public AudioClip opencloseInventorySound;
    public AudioClip unlockSkillSound;

    [Header("Settings")]
    public float footstepInterval = 0.5f;
    public float footstepTimer = 0f;

    private AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopLoop()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
    public void PlayFootstep(bool isRunning)
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            AudioClip clip = isRunning ? runClip : footstepClip;

            if (clip != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f); // เพิ่มความหลากหลาย
                PlayOneShot(clip);
            }

            footstepTimer = isRunning ? 0.25f : footstepInterval;
        }
    }
}
