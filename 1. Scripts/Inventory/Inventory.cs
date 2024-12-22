using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private ItemSlot[] itemSlots = new ItemSlot[4];

    [Header("Selected Item")]
    [SerializeField] private int selectedIndex;

    private void Start()
    {
        InventoryManager.Instance.Inventory = this;
        InitItemSlot();
        selectedIndex = 0;
    } 

    private void InitItemSlot()
    {
        uint index = 0;
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.Init(index++);
        }
        itemSlots[selectedIndex].SelectSlot();
    }

    public void AddItemRequest(Item item)
    {
        // 현재 선택된 인덱스 칸에 우선적으로 넣기
        if (!itemSlots[selectedIndex].IsFull())
        {
            GameManager.Instance.Player.NetworkHandler.ItemGetRequest(item.Id, selectedIndex);        
            return;
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (!itemSlots[i].IsFull())
            {
                GameManager.Instance.Player.NetworkHandler.ItemGetRequest(item.Id, i);
                return;
            }
        }
    }

    // Item Get Response와 연동
    public void AddItem(Item item, int index)
    {
        itemSlots[index].Add(item);
        GameManager.Instance.Player.ItemHoldHandler.SetItemTransform(item, item.Id);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.PickUpItem, GameManager.Instance.Player.transform.position);
        
        if (index == selectedIndex)
        {
            // 아이템 먹은 동시에 들기
            GameManager.Instance.Player.ItemHoldHandler.HoldItem(item.Id, true);
            itemSlots[index].SetDurationTXT(true);
            GameManager.Instance.Player.NetworkHandler.ItemChangeRequest(index);
        }
    }

    public void ThrowItemRequest()
    {
        // 현재 선택된 인덱스 칸에 아이템 있는 경우 LocalThrow
        if (itemSlots[selectedIndex].IsFull())
        {
            GameManager.Instance.Player.NetworkHandler.ItemDiscardRequest(itemSlots[selectedIndex].Item, selectedIndex);
        }
    }

    // Item Discard Response와 연동
    public void ThrowItem(int index)
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ThrowItem, GameManager.Instance.Player.transform.position);
        itemSlots[index].Throw();
    }

    // 인벤토리 꽉찼는지 여부
    public bool IsInventoryFull()
    {
        int cnt = 0;
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.IsFull())
                cnt++;
        }
        if (cnt >= itemSlots.Length)
            return true;
        return false;
    }

    public void ChangeSelectedItemRequest(bool isLeft)
    {
        itemSlots[selectedIndex].DeSelectSlot();
        if (isLeft)
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = itemSlots.Length - 1;
        }
        else
        {
            selectedIndex++;
            if (selectedIndex >= itemSlots.Length)
                selectedIndex = 0;
        }

        selectedIndex %= itemSlots.Length;
        itemSlots[selectedIndex].SelectSlot();
        GameManager.Instance.Player.NetworkHandler.ItemChangeRequest(selectedIndex);
    }
    
    public void UseItemRequest()
    {
        ItemSlot selectedSlot = itemSlots[selectedIndex];
        // 현재 선택된 인덱스 칸에 아이템이 있고, 사용 가능한 경우
        if (selectedSlot.IsFull() && selectedSlot.CanUse())
        {
            selectedSlot.CheckItemUseType();    
        }
    }

    // Item Use Response와 연동
    public void UseItem(uint index)
    {
        itemSlots[index].UseItem();
    }

    // Item DisUse Noti와 연동 => 손에 든 아이템만 DisUse 가능하므로 itemSlot에서 검사
    public void DisuseItem(uint itemId)
    {
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.IsSameItem(itemId))
                itemSlot.DisUseItem();
        }
    }

    // Item Delete Sync와 연동
    public void DeleteItem(uint itemId)
    {
        foreach (var itemSlot in itemSlots)
        {
            if(itemSlot.IsSameItem(itemId))
                itemSlot.Delete();
        }
    }
}
