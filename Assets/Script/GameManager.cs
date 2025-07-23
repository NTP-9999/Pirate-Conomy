using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        SetPlayerData();
    }
    public void SetPlayerData()
    {
        InventoryManager.Instance.items = PlayerData.Instance.items;
        ShipEnterExit.Instance.isControlling = PlayerData.Instance.sailing;
        // TODO: Set inventory items here, e.g. InventoryManager.Instance.SetItems(PlayerData.Instance.inventoryItems);
        PlayerCurrency.Instance.currentNova = PlayerData.Instance.money;
    }
    public void SavePlayerData()
    {
        PlayerData.Instance.items = InventoryManager.Instance.items;
        PlayerData.Instance.sailing = ShipEnterExit.Instance.isControlling;
        // TODO: Save inventory items here, e.g. PlayerData.Instance.inventoryItems = InventoryManager.Instance.GetItems();
        PlayerData.Instance.money = PlayerCurrency.Instance.currentNova;
    }
}
