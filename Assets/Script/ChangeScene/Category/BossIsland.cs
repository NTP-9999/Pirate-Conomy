using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossIsland : SceneChanger
{
    [Header("Boss Island Settings")]
    [SerializeField] private string bossName = "Boss";
    [SerializeField] private int recommendedLevel = 10;
    [SerializeField] private GameObject warningUI;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    private bool showingWarning = false;
    
    protected override void Start()
    {
        base.Start();
        
        // Setup warning UI buttons
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmBossFight);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelBossFight);
        }
    }
    
    protected override void ChangeScene()
    {
        // Show warning for boss islands
        if (warningUI != null)
        {
            ShowBossWarning();
        }
        else
        {
            // If no warning UI, proceed directly
            ProceedToBossFight();
        }
    }
    
    private void ShowBossWarning()
    {
        if (!showingWarning)
        {
            showingWarning = true;
            warningUI.SetActive(true);
            
            // Check player level (you can implement this based on your player stats system)
            CheckPlayerReadiness();
        }
    }
    
    private void CheckPlayerReadiness()
    {
        // Get player level from your stats system
        int playerLevel = GetPlayerLevel();
        
        if (playerLevel < recommendedLevel)
        {
            Debug.LogWarning($"Player level {playerLevel} is below recommended level {recommendedLevel} for {bossName}");
            // You can show additional warning text here
        }
    }
    
    private int GetPlayerLevel()
    {
        // Implement based on your player stats system
        // For now, return a default value
        return 1;
    }
    
    private void ConfirmBossFight()
    {
        ProceedToBossFight();
        HideWarning();
    }
    
    private void CancelBossFight()
    {
        HideWarning();
    }
    
    private void ProceedToBossFight()
    {
        Debug.Log($"Entering {bossName} boss fight...");
        string sceneToLoad = useStaticNextScene ? GetStaticSceneName() : targetSceneName;
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    private void HideWarning()
    {
        showingWarning = false;
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(ConfirmBossFight);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(CancelBossFight);
        }
    }
} 