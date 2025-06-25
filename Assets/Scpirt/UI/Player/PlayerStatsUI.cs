using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI HPValue;
    public TextMeshProUGUI STAValue;
    public TextMeshProUGUI HUNValue;
    public TextMeshProUGUI SPValue;

    private CharacterStats characterStats;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterStats = player.GetComponent<CharacterStats>();
        }
        else
        {
            Debug.LogError("Player not found! Make sure the Player has tag 'Player'");
        }
    }

    void Update()
    {
        if (characterStats == null) return;

        HPValue.text  = $"{characterStats.currentHealth:0}";
        STAValue.text = $"{characterStats.currentStamina:0}";
        HUNValue.text  = $"{characterStats.currentHunger:0}";
        SPValue.text  = $"{characterStats.currentStress:0}";
    }
}
