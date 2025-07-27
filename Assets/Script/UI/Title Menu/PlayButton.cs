using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Video;

public class PlayButton : MonoBehaviour
{
    public static string nextScene = "pirate"; // ชื่อซีนที่ต้องการไปหลัง loading
    public GameObject cutScene;
    public VideoPlayer videoPlayer;

    public void Playgame()
    {
        StartCoroutine(FadeOutBGM());
        StartCoroutine(KuyKuy());
        StartCoroutine(WaitForCutScene());
    }
    IEnumerator KuyKuy()
    {
        yield return new WaitForSeconds(4f); // รอ 1 วินาที
        cutScene.SetActive(true); // เปิด Cutscene
        videoPlayer.Play(); // เล่นวิดีโอ
    }
    IEnumerator WaitForCutScene()
    {
        yield return new WaitForSeconds(44f);
        SceneManager.LoadScene("Loading_screen");
    }
    IEnumerator FadeOutBGM()
    {
        GameObject bgmObject = GameObject.Find("BGM");
        if (bgmObject != null)
        {
            AudioSource bgmAudio = bgmObject.GetComponent<AudioSource>();
            if (bgmAudio != null)
            {
                float duration = 3f; // เวลาในการ fade (วินาที)
                float startVolume = bgmAudio.volume;

                float t = 0f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    bgmAudio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                    yield return null;
                }

                bgmAudio.volume = 0f; // เผื่อบางเครื่องมี Lerp ไม่พอดีเป๊ะ
            }
        }
    }
}
