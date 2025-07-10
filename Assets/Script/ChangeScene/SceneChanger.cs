using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] protected string targetSceneName;
    [SerializeField] protected bool useStaticNextScene = false;
    
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
        if (other.CompareTag("Player"))
        {
            ChangeScene();
        }
    }
    
    protected virtual void ChangeScene()
    {
        string sceneToLoad = useStaticNextScene ? GetStaticSceneName() : targetSceneName;
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Loading scene: {sceneToLoad}");
            
            // Use SceneTransitionManager if available, otherwise fallback to direct loading
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            Debug.LogError($"No scene name specified for {gameObject.name}");
        }
    }
    
    protected virtual string GetStaticSceneName()
    {
        return targetSceneName; // Override in derived classes if needed
    }
} 