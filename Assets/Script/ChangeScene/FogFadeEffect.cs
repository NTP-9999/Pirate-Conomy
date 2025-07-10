using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FogFadeEffect : MonoBehaviour
{
    public Image fogImage; // ใส่ UI Image สีหมอก
    private float fadeTime;

    public IEnumerator FadeIn(float fadeTime)
    {
        fogImage.gameObject.SetActive(true);
        Color c = fogImage.color;
        c.a = 0f;
        fogImage.color = c;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeTime);
            fogImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fogImage.color = c;
    }

    public IEnumerator FadeOut()
    {
        fogImage.gameObject.SetActive(true);
        Color c = fogImage.color;
        c.a = 1f;
        fogImage.color = c;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(t / fadeTime);
            fogImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fogImage.color = c;
        fogImage.gameObject.SetActive(false);
    }
}