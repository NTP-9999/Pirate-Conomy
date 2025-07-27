using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("SFX Clips")]
    public AudioClip footstepClip;
    public AudioClip runClip;
    public AudioClip rollClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip attackClip;
    public AudioClip blockClip;
    public AudioClip parryClip;
    public AudioClip skillCastClip;

    [Header("Settings")]
    public float footstepInterval = 0.5f;

    private AudioSource audioSource;
    private float footstepTimer;

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
            PlayOneShot(clip);
            footstepTimer = isRunning ? 0.3f : footstepInterval;
        }
    }
}
