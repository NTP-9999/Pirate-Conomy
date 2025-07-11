using UnityEngine;
using System.Collections.Generic;

public class PlayerData : Singleton<PlayerData>
{
    public Vector3 playerposition;
    public Vector3 shipPosition;
    public bool sailing;
    public int money;
    public int currenthealth;  
    public List<InventoryItem> items = new List<InventoryItem>();
}
