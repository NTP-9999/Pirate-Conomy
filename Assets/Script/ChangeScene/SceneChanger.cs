using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public abstract class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] protected string targetSceneName;
    [SerializeField] protected bool useStaticNextScene = false;
    [SerializeField] protected FogFadeEffect fogEffect; // ใส่ใน Inspector
    [SerializeField] protected float fogFadeTime = 1.0f;
    
    protected virtual void Start()
    {
        // Ensure we have a collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError($"SceneChanger on {gameObject.name} needs a Collider component!");
        }
        
        // Set as trigger if it's not already
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            ChangeScene();
        }
    }
    
    protected virtual void ChangeScene()
    {
        if (fogEffect != null)
        {
            StartCoroutine(FadeAndChangeScene());
        }
        else
        {
            // ถ้าไม่มี effect ให้เปลี่ยน scene ทันที
            LoadingScreenData.nextScene = useStaticNextScene ? GetStaticSceneName() : targetSceneName;
            SceneManager.LoadScene("Loading_screen");
        }
    }

    private IEnumerator FadeAndChangeScene()
    {
        yield return StartCoroutine(fogEffect.FadeIn(fogFadeTime));
        LoadingScreenData.nextScene = useStaticNextScene ? GetStaticSceneName() : targetSceneName;
        SceneManager.LoadScene("Loading_screen");
    }
    
    protected virtual string GetStaticSceneName()
    {
        return targetSceneName; // Override in derived classes if needed
    }
} 