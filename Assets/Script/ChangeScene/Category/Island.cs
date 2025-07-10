using UnityEngine;

public class Island : SceneChanger
{
    [Header("Main Island Settings")]
    [SerializeField] private string islandName = "Main Island";
    
    protected override void Start()
    {
        // Set the target scene name based on the island name
        targetSceneName = islandName;
        base.Start();
    }
    
    // Optional: Override to add custom logic for main islands
    protected override void ChangeScene()
    {
        Debug.Log($"Traveling to Main Island: {islandName}...");
        base.ChangeScene();
    }
} 