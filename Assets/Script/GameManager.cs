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
        // Only set player position if it's not the default (0,0,0) position
        if (playerController != null && PlayerData.Instance.playerposition != Vector3.zero)
        {
            playerController.transform.position = PlayerData.Instance.playerposition;
        }

        // ตำแหน่งเรือ (ถ้ามี)
        if (PlayerData.Instance.shipPosition != Vector3.zero)
        {
            ShipController.Instance.shipPosition = PlayerData.Instance.shipPosition;
        }
        InventoryManager.Instance.items = PlayerData.Instance.items;
        ShipEnterExit.Instance.isControlling = PlayerData.Instance.sailing;
        // TODO: Set inventory items here, e.g. InventoryManager.Instance.SetItems(PlayerData.Instance.inventoryItems);
        PlayerCurrency.Instance.currentNova = PlayerData.Instance.money;
    }
    public void SavePlayerData()
    {
        PlayerData.Instance.items = InventoryManager.Instance.items;
        if (playerController != null)
        {
            PlayerData.Instance.playerposition = playerController.transform.position;
        }
        PlayerData.Instance.sailing = ShipEnterExit.Instance.isControlling;
        PlayerData.Instance.shipPosition = ShipController.Instance.shipPosition;
        // TODO: Save inventory items here, e.g. PlayerData.Instance.inventoryItems = InventoryManager.Instance.GetItems();
        PlayerData.Instance.money = PlayerCurrency.Instance.currentNova;
    }
}
