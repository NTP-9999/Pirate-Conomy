using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Image progressImage; // ลาก Image ที่ตั้งค่าเป็น Filled มาใส่ใน Inspector

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingScreenData.nextSceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // คำนวณ progress ระหว่าง 0 - 1
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressImage != null)
                progressImage.fillAmount = progress;

            // เมื่อโหลดถึง 90%
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
