using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    // UI Elements
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    
    // Buttons
    private Button buyBtn;

    // Data
    private uint itemTypeId;

    public void Init(int itemTypeId)
    {
        buyBtn = GetComponent<Button>();
        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(ItemPurchaseRequest);
        thumbnail.sprite = DataManager.Instance.ItemDataDictionary[itemTypeId].ItemData.Sprite;
        nameTxt.text = DataManager.Instance.ItemDataDictionary[itemTypeId].ItemData.ItemSO.Name;
        priceTxt.text = $"Price : {DataManager.Instance.ItemDataDictionary[itemTypeId].ItemData.ItemSO.Price}";
        this.itemTypeId = (uint)itemTypeId;
    }

    private void ItemPurchaseRequest()
    {
        GamePacket packet = new GamePacket();
        packet.ItemPurchaseRequest = new C2S_ItemPurchaseRequest();
        packet.ItemPurchaseRequest.ItemTypeId = itemTypeId;
        GameServerSocketManager.Instance.Send(packet);
    }
}