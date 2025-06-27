using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class LoadingManager : MonoBehaviour
{
    [Header("Loading Settings")]
    public string sceneToLoad; 
    public float minimumLoadingTime = 2f;

    [Header("UI Elements (Optional)")]
    public GameObject loadingScreenUI;

    private float startTime;

    void Start()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("LoadingManager: sceneToLoad is not set! Please specify the name of the scene to load.", this);
            return;
        }

        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(true);
        }

        startTime = Time.time;
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; 

        while (!operation.isDone || Time.time - startTime < minimumLoadingTime)
        {
            if (operation.isDone && Time.time - startTime >= minimumLoadingTime)
            {
                break;
            }
            yield return null;
        }

        operation.allowSceneActivation = true; 
        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(false); 
        }
    }
}