using UnityEngine;
using System.Collections;
public class NagaAudioManager : Singleton<NagaAudioManager>
{

    public AudioClip hurtClip;
    public AudioClip poisonClip;
    public AudioClip tornadoClip;
    public AudioClip bgmClip;
    public AudioClip dieClip;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;


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
    public void PlayLoop(AudioClip clip, float fadeInDuration = 0.2f)
    {
        // ถ้ามีกำลังลดเสียง ก็ควรหยุดก่อน
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = 0f;
            audioSource.Play();
            // ค่อยๆ เพิ่มเสียงขึ้นจนเต็ม
            fadeCoroutine = StartCoroutine(FadeVolume(0f, 1f, fadeInDuration));
        }
    }

    /// <summary>
    /// ค่อยๆ ลด volume แล้ว Stop เสียง
    /// </summary>
    /// <param name="fadeOutDuration">ระยะเวลาในการ Fade out (วินาที)</param>
    public void StopLoop(float fadeOutDuration = 0.5f)
    {
        if (audioSource.isPlaying)
        {
            // ถ้ามี Coroutine เก่า ให้หยุดก่อน
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeOutDuration));
        }
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.volume = startVolume; // คืนค่าเดิมเผื่อเล่นครั้งถัดไป
        fadeCoroutine = null;
    }

    /// <summary>
    /// Coroutine ทั่วไปสำหรับ Fade volume
    /// </summary>
    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        audioSource.volume = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        audioSource.volume = to;
        fadeCoroutine = null;
    }
}