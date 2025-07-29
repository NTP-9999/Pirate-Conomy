using UnityEngine;

public class KravalonAudioManager : Singleton<KravalonAudioManager>
{
    public AudioClip dieSFX;
    public AudioClip spawnSFX;
    private AudioSource audioSource;
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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

}