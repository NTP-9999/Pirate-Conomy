using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NagaAudioManager : Singleton<NagaAudioManager>
{
    public AudioClip hurtClip;
    public AudioClip poisonClip;
    public AudioClip tornadoClip;
    public AudioClip bgmClip;
    public AudioClip dieClip;

    // AudioSource สำหรับ Loop/BGM
    private AudioSource loopSource;
    private Coroutine loopFadeCoroutine;

    // AudioSource เดียวสำหรับ PlayOneShot ปกติ
    private AudioSource audioSource;

    // List เก็บ AudioSource ของ One‑Shot แบบ Smooth
    private List<AudioSource> oneShotSources = new List<AudioSource>();

    [Header("Volumes")]
    [Range(0,1)] public float bgmVolume      = 0.6f;
    [Range(0,1)] public float sfxLoopVolume  = 1f;

    // Two separate sources:
    private AudioSource bgmSource;
    private Coroutine   bgmFadeCoroutine;
    private Coroutine   sfxLoopFadeCoroutine;
    private AudioSource sfxLoopSource;


    void Awake()
    {
        // 1) ตั้ง loopSource
        var existing = GetComponent<AudioSource>();
        if (existing == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            loopSource = existing;
        }
        loopSource.playOnAwake = false;

        // 2) เพิ่ม audioSource สำหรับ PlayOneShot ปกติ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        sfxLoopSource = gameObject.AddComponent<AudioSource>();
        sfxLoopSource.playOnAwake = false;
        sfxLoopSource.loop = true;
        sfxLoopSource.volume = sfxLoopVolume;
        
    }
    /// <summary>Fade‑in a BGM clip.</summary>
    public void PlayBGM(AudioClip clip, float fadeInDuration = 1f)
    {
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);

        if (bgmSource.clip != clip)
        {
            bgmSource.clip   = clip;
            bgmSource.volume = 0f;
            bgmSource.Play();
            bgmFadeCoroutine = StartCoroutine(
                FadeVolume(bgmSource, 0f, bgmVolume, fadeInDuration)
            );
        }
    }
    /// <summary>Fade‑out & stop the current BGM.</summary>
    public void StopBGM(float fadeOutDuration = 1f)
    {
        if (!bgmSource.isPlaying) return;
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(
            FadeOutAndStop(bgmSource, fadeOutDuration, resetClip: true)
        );
    }
    // ─── SFX LOOP ───────────────────────────────────────────────────────────────

    /// <summary>Fade‑in a looping SFX (e.g. tornado).</summary>
    public void PlaySfxLoop(AudioClip clip, float fadeInDuration = 0.3f)
    {
        if (sfxLoopFadeCoroutine != null) StopCoroutine(sfxLoopFadeCoroutine);

        if (sfxLoopSource.clip != clip)
        {
            sfxLoopSource.clip   = clip;
            sfxLoopSource.volume = 0f;
            sfxLoopSource.Play();
            sfxLoopFadeCoroutine = StartCoroutine(
                FadeVolume(sfxLoopSource, 0f, sfxLoopVolume, fadeInDuration)
            );
        }
    }

    /// <summary>Fade‑out & stop the looping SFX.</summary>
    public void StopSfxLoop(float fadeOutDuration = 1f)
    {
        if (!sfxLoopSource.isPlaying) return;
        if (sfxLoopFadeCoroutine != null) StopCoroutine(sfxLoopFadeCoroutine);

        sfxLoopFadeCoroutine = StartCoroutine(
            FadeOutAndStop(sfxLoopSource, fadeOutDuration, resetClip: true)
        );
    }


    // ─── NORMAL ONE‑SHOT ────────────────────────────────────────────────────────

    /// <summary>ใช้เสียง SFX แบบเดิม (ไม่มี fade)</summary>
    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // ─── SMOOTH ONE‑SHOT ───────────────────────────────────────────────────────

    /// <summary>เล่น one‑shot บน AudioSource ใหม่ เพื่อเตรียม fade ออกได้</summary>
    public void PlayOneShotSmooth(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        var go = new GameObject("OneShot_" + clip.name);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volumeScale;
        src.loop = false;
        src.Play();

        oneShotSources.Add(src);
    }

    /// <summary>fade‑out และทำลาย AudioSource ของ one‑shot นั้น</summary>
    public void StopOneShotSmooth(AudioClip clip, float fadeOutDuration = 0.5f)
    {
        var src = oneShotSources.Find(s => s.clip == clip && s.isPlaying);
        if (src != null)
            StartCoroutine(FadeOutAndStop(src, fadeOutDuration, destroyWhenDone: true));
    }

    // ─── LOOP / BGM ──────────────────────────────────────────────────────────────

    /// <summary>fade‑in แล้ว loop เสียง</summary>
    public void PlayLoop(AudioClip clip, float fadeInDuration = 0.2f)
    {
        if (loopFadeCoroutine != null)
            StopCoroutine(loopFadeCoroutine);

        if (loopSource.clip != clip)
        {
            loopSource.clip = clip;
            loopSource.loop = true;
            loopSource.volume = 0f;
            loopSource.Play();

            loopFadeCoroutine = StartCoroutine(
                FadeVolume(loopSource, 0f, 1f, fadeInDuration)
            );
        }
    }

    /// <summary>fade‑out แล้ว Stop loop</summary>
    public void StopLoop(float fadeOutDuration = 0.5f)
    {
        if (!loopSource.isPlaying) return;

        if (loopFadeCoroutine != null)
            StopCoroutine(loopFadeCoroutine);

        loopFadeCoroutine = StartCoroutine(
            FadeOutAndStop(loopSource, fadeOutDuration, resetClip: true)
        );
    }

    // ─── COMMON COROUTINES ───────────────────────────────────────────────────────

    /// <summary>fade volume จากค่า from → to ในเวลาที่กำหนด</summary>
    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        float elapsed = 0f;
        source.volume = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        source.volume = to;
    }

    /// <summary>
    /// fade‑out แล้ว Stop AudioSource 
    /// ถ้า resetClip=true จะล้าง clip, 
    /// ถ้า destroyWhenDone=true จะลบ GameObject ทิ้ง
    /// </summary>
    private IEnumerator FadeOutAndStop(
        AudioSource source,
        float duration,
        bool resetClip = false,
        bool destroyWhenDone = false
    )
    {
        float startVol = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        if (resetClip) source.clip = null;
        source.volume = startVol;

        if (destroyWhenDone)
        {
            oneShotSources.Remove(source);
            Destroy(source.gameObject);
        }
    }
}
