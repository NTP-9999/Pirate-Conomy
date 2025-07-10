using UnityEngine;

public class SmallIsland : SceneChanger
{
    [Header("Small Island Settings")]
    [SerializeField] private string islandName = "Small Island";
    
    protected override void Start()
    {
        // Set the target scene name based on the island name
        targetSceneName = islandName;
        base.Start();
    }
    
    // Optional: Override to add custom logic for small islands
    protected override void ChangeScene()
    {
        Debug.Log($"Traveling to Small Island: {islandName}...");
        base.ChangeScene();
    }
} 