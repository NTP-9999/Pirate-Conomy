using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public Image fadePanel; // ลาก FadePanel (Image) ที่สร้างไว้ใน Canvas มาใส่ใน Inspector
    public float fadeDuration = 2.0f; // ระยะเวลาในการจอดำ

    private bool isFading = false;

    public void StartFadeToBlack()
    {
        if (isFading) return;

        isFading = true;
        fadePanel.gameObject.SetActive(true); // ทำให้ Panel แสดงผล
        StartCoroutine(FadeToBlackRoutine());
    }

    private IEnumerator FadeToBlackRoutine()
    {
        float timer = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // เปลี่ยน alpha เป็น 1 (ทึบ)

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / fadeDuration;
            fadePanel.color = Color.Lerp(startColor, targetColor, normalizedTime);
            yield return null;
        }

        fadePanel.color = targetColor;
        isFading = false;
        Debug.Log("Fade to black complete.");

        // ตัวเลือก: เมื่อจอดำสนิทแล้ว อาจจะโหลดฉาก Game Over หรือแสดงปุ่ม Restart
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }

    // ฟังก์ชันเสริมสำหรับ Fade Out (จากดำกลับมา) ถ้าคุณต้องการใช้ในอนาคต
    public void StartFadeFromBlack()
    {
        if (isFading) return;

        isFading = true;
        fadePanel.gameObject.SetActive(true); // ทำให้ Panel แสดงผล (ถ้าถูกปิดไว้)
        StartCoroutine(FadeFromBlackRoutine());
    }

    private IEnumerator FadeFromBlackRoutine()
    {
        float timer = 0f;
        Color startColor = fadePanel.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / fadeDuration;
            fadePanel.color = Color.Lerp(startColor, targetColor, normalizedTime);
            yield return null;
        }

        fadePanel.color = targetColor;
        isFading = false;
        fadePanel.gameObject.SetActive(false); // ซ่อน Panel เมื่อ Fade Out เสร็จ
    }
}