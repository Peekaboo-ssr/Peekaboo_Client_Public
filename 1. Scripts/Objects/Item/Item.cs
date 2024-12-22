using System;
using UnityEngine;

// 데이터 위치 추후 옮기기
[Serializable]
public class ItemData
{
    public EItemType Type;
    public ItemBaseSO ItemSO;
    public Sprite Sprite;
}

public class Item : MonoBehaviour
{
    [field: SerializeField] public uint Id { get; private set; }
    [field: SerializeField] public ItemData ItemData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Vector3 HoldPosition { get; private set; }
    [field: SerializeField] public Vector3 HoldRotation { get; private set; }
    [field: SerializeField] public bool IsInteractable { get; private set; }

    private int playerLayer;
    private int itemLayer;

    protected virtual void Awake()
    {
        Rigidbody = GetComponentInChildren<Rigidbody>();

        playerLayer = LayerMask.NameToLayer("Player");
        itemLayer = LayerMask.NameToLayer("Item");
        IsInteractable = true;
    }

    public void Init(uint id)
    {
        Id = id;
        IsInteractable = true;
    }

    #region Physics
    public void SetKinematic(bool isKinematic)
    {
        Rigidbody.isKinematic = isKinematic;
    }

    public void SetPlayerItemCollisionIgnore(bool isIgnore)
    {
        Physics.IgnoreLayerCollision(playerLayer, itemLayer, isIgnore);
    }
    #endregion

    public virtual void LocalPickUp()
    {
        if (InventoryManager.Instance.Inventory.IsInventoryFull())
            return;

        SetKinematic(true);
        SetPlayerItemCollisionIgnore(true);

        InventoryManager.Instance.Inventory.AddItemRequest(this);
        IsInteractable = false;
    }

    public virtual void LocalThrow()
    {
        SetKinematic(false);
        IsInteractable = true;
    }

    public virtual void RemotePickUp(RemotePlayer remotePlayer)
    {
        SetKinematic(true);
        SetPlayerItemCollisionIgnore(true);
        IsInteractable = false;
    }

    public virtual void RemoteThrow(RemotePlayer remotePlayer)
    {
        SetKinematic(false);
        IsInteractable = true;
    }

    public virtual void Destroy()
    {

    }
}
