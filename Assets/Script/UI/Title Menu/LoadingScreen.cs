using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // ใส่ Slider UI ใน Inspector

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(PlayButton.nextScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // คำนวณ progress ระหว่าง 0 - 1
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            // เมื่อโหลดถึง 90% (Unity จะหยุดรอให้ allowSceneActivation เป็น true)
            if (operation.progress >= 0.9f)
            {
                // หน่วงเวลาเล็กน้อยให้ผู้เล่นเห็น progress 100%
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
