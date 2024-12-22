using System;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [field:SerializeField] public Inventory Inventory { get; set; }
    public void ExtractSoulRequest()
    {
        GamePacket packet = new GamePacket();
        packet.ExtractSoulRequest = new C2S_ExtractSoulRequest();
        //packet.ExtractSoulRequest.UserId = NetworkManager.Instance.UserId;
    }
}