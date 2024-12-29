using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHoldHandler : MonoBehaviour
{
    [Header("Item Hold")]
    [field: SerializeField] public Transform ItemHoldTransform { get; private set; }
    private Dictionary<uint, Item> holdItems = new Dictionary<uint, Item>();

    // 아이템 플레이어 손에 위치시키고 Dictionary에 추가
    public void SetItemTransform(Item obj, uint itemId)
    {
        obj.transform.SetParent(ItemHoldTransform, false);

        obj.transform.localPosition = obj.HoldPosition;
        obj.transform.localRotation = Quaternion.Euler(obj.HoldRotation);

        if (!holdItems.ContainsKey(itemId))
            holdItems[itemId] = obj;
        obj.gameObject.SetActive(false);
    }

    // 아이템 들기 & 안들기
    public void HoldItem(uint itemId, bool isHold)
    {
        if (holdItems.ContainsKey(itemId))
        {
            holdItems[itemId].gameObject.SetActive(isHold);
        }
    }

    public void ThrowItem(uint itemId, Transform dropPosition)
    {
        if (holdItems.ContainsKey(itemId))
        {
            Item obj = holdItems[itemId];
            holdItems.Remove(itemId);

            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null, true);

            obj.Rigidbody.linearVelocity = Vector3.zero;
            obj.Rigidbody.angularVelocity = Vector3.zero;
            obj.Rigidbody.MovePosition(dropPosition.position);
        }
    }

    public void DeleteItem(uint itemId)
    {
        if (holdItems.ContainsKey(itemId))
        {
            Item obj = holdItems[itemId];
            holdItems.Remove(itemId);
            obj.Destroy();
        }
    }

    public void Barehand()
    {
        foreach(var key in holdItems.Keys)
        {
            HoldItem(holdItems[key].Id, false);
        }
    }
}
