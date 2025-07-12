using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        SetPlayerData();
        Transform startingTarget = GameObject.Find("Target1").transform;
        var firstQuest = new TalkToNPCQuest("Tutorial", "Talk To Merchant", startingTarget);
        QuestManager.Instance.AddQuest(firstQuest, autoStart: true);
    }
    public void SetPlayerData()
    {
        // Only set player position if it's not the default (0,0,0) position
        if (PlayerData.Instance.playerposition != Vector3.zero)
        {
            CharacterMovement.Instance.PlayerPosition = PlayerData.Instance.playerposition;
        }
        if (PlayerData.Instance.shipPosition != Vector3.zero)
        {
            ShipController.Instance.shipPosition = PlayerData.Instance.shipPosition;
        }
        ShipEnterExit.Instance.isControlling = PlayerData.Instance.sailing;
        // TODO: Set inventory items here, e.g. InventoryManager.Instance.SetItems(PlayerData.Instance.inventoryItems);
        PlayerCurrency.Instance.currentNova = PlayerData.Instance.money;
        CharacterStats.Instance.currentHealth = PlayerData.Instance.currenthealth;
    }
    public void SavePlayerData()
    {
        PlayerData.Instance.playerposition = CharacterMovement.Instance.PlayerPosition;
        PlayerData.Instance.sailing = ShipEnterExit.Instance.isControlling;
        PlayerData.Instance.shipPosition = ShipController.Instance.shipPosition;
        // TODO: Save inventory items here, e.g. PlayerData.Instance.inventoryItems = InventoryManager.Instance.GetItems();
        PlayerData.Instance.money = PlayerCurrency.Instance.currentNova;
        PlayerData.Instance.currenthealth = (int)CharacterStats.Instance.currentHealth;
    }
}
