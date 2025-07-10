using UnityEngine;

public class PlayerIsland : SceneChanger
{
    [Header("Player Island Settings")]
    [SerializeField] private string playerIslandName = "Pirate Island";
    
    protected override void Start()
    {
        // Set the target scene name based on the player island name
        targetSceneName = playerIslandName;
        base.Start();
    }
    
    // Optional: Override to add custom logic for player island
    protected override void ChangeScene()
    {
        Debug.Log($"Returning to Player Island: {playerIslandName}...");
        base.ChangeScene();
    }
    
    // Special method for player island - can add home-specific logic
    protected virtual void OnReturnToHome()
    {
        // Add any special logic when returning to player island
        // For example: Save game, reset health, etc.
        Debug.Log("Welcome back to your home island!");
    }
} 