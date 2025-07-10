using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    
    [Header("Transition Settings")]
    [SerializeField] private GameObject loadingScreenPrefab;
    [SerializeField] private float minimumLoadTime = 1f;
    [SerializeField] private bool showLoadingScreen = true;
    
    private bool isTransitioning = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }
    
    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isTransitioning = true;
        
        // Show loading screen if enabled
        GameObject loadingScreen = null;
        if (showLoadingScreen && loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
        }
        
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        float startTime = Time.time;
        
        // Wait for scene to load
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        // Ensure minimum load time
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadTime)
        {
            yield return new WaitForSeconds(minimumLoadTime - elapsedTime);
        }
        
        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait for scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Remove loading screen
        if (loadingScreen != null)
        {
            Destroy(loadingScreen);
        }
        
        isTransitioning = false;
    }
    
    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        isTransitioning = true;
        
        // Show loading screen if enabled
        GameObject loadingScreen = null;
        if (showLoadingScreen && loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
        }
        
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;
        
        float startTime = Time.time;
        
        // Wait for scene to load
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        // Ensure minimum load time
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadTime)
        {
            yield return new WaitForSeconds(minimumLoadTime - elapsedTime);
        }
        
        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait for scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Remove loading screen
        if (loadingScreen != null)
        {
            Destroy(loadingScreen);
        }
        
        isTransitioning = false;
    }
    
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
} 