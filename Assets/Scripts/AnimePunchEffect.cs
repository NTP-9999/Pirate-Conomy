
using UnityEngine;

public class AnimePunchEffect : MonoBehaviour
{
    public ParticleSystem windEffect;
    public AudioSource whooshSound;
    public CameraShake camShake;

    public void Trigger()
    {
        if (windEffect != null) windEffect.Play();
        if (whooshSound != null) whooshSound.Play();
        if (camShake != null) StartCoroutine(camShake.Shake(0.15f, 0.2f));
        TriggerSlowMo(0.2f, 0.2f);
    }

    private void TriggerSlowMo(float duration, float slowTimeScale)
    {
        StartCoroutine(SlowMoRoutine(duration, slowTimeScale));
    }

    private System.Collections.IEnumerator SlowMoRoutine(float duration, float slowTimeScale)
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * slowTimeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
