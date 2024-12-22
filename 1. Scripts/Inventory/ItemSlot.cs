using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("Slot Data")]
    [field:SerializeField] public Item Item { get; private set; }
    [SerializeField] private uint index;
    [SerializeField] private bool isFull;

    [Header("Slot UI")]
    [SerializeField] private Image itemImage;
    [SerializeField] private Sprite itemNullImage;
    [SerializeField] private Image slotBG;
    [SerializeField] private Color originColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private TextMeshProUGUI TXT_duration;

    // 슬롯 초기화
    public void Init(uint index)
    {
        this.index = index;
        itemImage.sprite = itemNullImage;
        Item = null;
        isFull = false;
        SetDurationTXT(false);
    }

    // 슬롯 채우기
    public void Add(Item item)
    {
        this.Item = item;
        itemImage.sprite = item.ItemData.Sprite;
        isFull = true;

        if (item.ItemData.ItemSO.IsUsable)
        {
            FlashLight flashLight = item as FlashLight;
            if (flashLight != null)
                flashLight.OnUseFlashLightEvent += UpdateDurationTXT;
            UpdateDurationTXT(flashLight.GetNowDuration());
        }
    }

    // 슬롯 비우기
    public void Throw()
    {
        // 플레이어가 들고있는 Item 목록에서 아이템 LocalThrow
        GameManager.Instance.Player.ItemHoldHandler.ThrowItem(Item.Id, GameManager.Instance.Player.ItemHoldHandler.ItemHoldTransform);

        itemImage.sprite = itemNullImage;
        isFull = false;
        SetDurationTXT(false);

        // 버릴 때 사용 중인 아이템 꺼주기
        if (Item.ItemData.ItemSO.IsUsable)
        {
            // 아이템 Disuse Request보내기
            GameManager.Instance.Player.NetworkHandler.ItemDisuseRequest(Item.Id);

            FlashLight flashLight = Item as FlashLight;
            if (flashLight != null)
            {
                flashLight.OnUseFlashLightEvent -= UpdateDurationTXT;
            }
        }
        Item.LocalThrow();
        Item = null;
    }

    public void Delete()
    {
        GameManager.Instance.Player.ItemHoldHandler.DeleteItem(Item.Id);

        itemImage.sprite = itemNullImage;
        isFull = false;
        SetDurationTXT(false);

        // 버릴 때 사용 중인 아이템 꺼주기
        if (Item.ItemData.ItemSO.IsUsable)
        {            
            // 아이템 Disuse Request보내기
            GameManager.Instance.Player.NetworkHandler.ItemDisuseRequest(Item.Id);

            FlashLight flashLight = Item as FlashLight;
            if (flashLight != null)
            {
                flashLight.OnUseFlashLightEvent -= UpdateDurationTXT;
            }
        }

        Item = null;
    }

    // 슬롯 선택
    public void SelectSlot()
    {
        slotBG.color = selectedColor;
        if (isFull)
        {
            GameManager.Instance.Player.ItemHoldHandler.HoldItem(Item.Id, true);
            // 아이템 들고있을 때, 애니메이션 모션 설정
            GameManager.Instance.Player.AnimationHandler.ActivateItemHoldLayer(GameManager.Instance.Player);
        }
        else
        {
            GameManager.Instance.Player.AnimationHandler.DeActivateItemHoldLayer(GameManager.Instance.Player);
        }
        SetDurationTXT(true);
    }
     
    // 슬롯 선택 해제
    public void DeSelectSlot()
    {
        slotBG.color = originColor;
        if (isFull)
        {
            GameManager.Instance.Player.ItemHoldHandler.HoldItem(Item.Id, false);
            if (Item.ItemData.ItemSO.IsUsable)
            {
                FlashLight flashLight = Item as FlashLight;
                if (flashLight != null)
                    flashLight.TurnOff();
            }
        }
        SetDurationTXT(false);
    }

    public void CheckItemUseType()
    {
        switch (Item.ItemData.Type)
        {
            case EItemType.ITM0001:
                FlashLight flashLight = Item as FlashLight;
                if (flashLight != null)
                    if(flashLight.IsFlashOn())
                        GameManager.Instance.Player.NetworkHandler.ItemDisuseRequest(Item.Id);
                    else
                        GameManager.Instance.Player.NetworkHandler.ItemUseRequest(index);
                break;
        }
    }

    // 아이템 사용
    public void UseItem()
    {
        switch(Item.ItemData.Type)
        {
            case EItemType.ITM0001:
                FlashLight flashLight = Item as FlashLight;
                if (flashLight != null)
                        flashLight.TurnOn();
                break;
        }
    }

    // 아이템 Disuse
    public void DisUseItem()
    {
        switch (Item.ItemData.Type)
        {
            case EItemType.ITM0001:
                FlashLight flashLight = Item as FlashLight;
                if (flashLight != null)
                        flashLight.TurnOff();
                break;
        }
    }

    public void SetDurationTXT(bool isActive)
    {
        if (isActive && Item != null && Item.ItemData.ItemSO.IsUsable)
        {
            TXT_duration.gameObject.SetActive(true);
            return;
        }
        TXT_duration.gameObject.SetActive(false);
    }

    public void UpdateDurationTXT(string str)
    {
        TXT_duration.text = str;
    }

    public bool IsFull()
    {
        return isFull;
    }

    public bool CanUse()
    {
        return Item.ItemData.ItemSO.IsUsable;
    }

    public bool IsSameItem(uint itemId)
    {
        if(Item != null)
            return Item.Id == itemId;
        return false;
    }
}
