using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{
    public Vector3 playerposition;
    public Vector3 shipPosition;
    public bool sailing;
    public int money;
    public int currenthealth;  
    public InventoryManager inventorymanager;
}
