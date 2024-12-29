using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerNetworkHandler : MonoBehaviour
{
    [SerializeField] private WaitForSeconds moveRQWaitForSeconds = new WaitForSeconds(0.1f);
    private Coroutine moveRQCoroutine;

    #region Move Request
    public void SendMoveRequest()
    {
        if (this == null) return;
        if (moveRQCoroutine != null)
            StopCoroutine(moveRQCoroutine);
        moveRQCoroutine = StartCoroutine(SendMoveRequestCorutine());
    }

    public void StopMoveRequest()
    {
        if (moveRQCoroutine != null)
        {
            StopCoroutine(moveRQCoroutine);
        }
            
    }

    private IEnumerator SendMoveRequestCorutine()
    {
        yield return new WaitUntil(() => GameManager.Instance.Player != null);
        while (true)
        {
            MoveRequest(GameManager.Instance.Player.transform.position, GameManager.Instance.Player.transform.rotation);
            yield return moveRQWaitForSeconds;
        }
    }

    private void MoveRequest(Vector3 position, Quaternion rotation)
    {
        GamePacket packet = new GamePacket();
        packet.PlayerMoveRequest = new C2S_PlayerMoveRequest();

        PlayerMoveInfo playerMoveInfo = new PlayerMoveInfo();

        Position pos = new Position();
        Rotation rot = new Rotation();

        pos.X = position.x;
        pos.Y = position.y;
        pos.Z = position.z;

        Vector3 eulerAngles = GameManager.Instance.Player.CameraTransform.rotation.eulerAngles;

        rot.X = eulerAngles.x;
        rot.Y = eulerAngles.y;
        rot.Z = eulerAngles.z;

        playerMoveInfo.UserId = NetworkManager.Instance.UserId;
        playerMoveInfo.Position = pos;
        playerMoveInfo.Rotation = rot;

        packet.PlayerMoveRequest.PlayerMoveInfo = playerMoveInfo;

        GameServerSocketManager.Instance.Send(packet);
    }

    #endregion

    #region Item Request
    // 아이템 Get 요청
    public void ItemGetRequest(uint itemId, int index)
    {
        GamePacket packet = new GamePacket();
        packet.ItemGetRequest = new C2S_ItemGetRequest();

        packet.ItemGetRequest.ItemId = itemId;
        packet.ItemGetRequest.InventorySlot = (uint)index + 1;

        GameServerSocketManager.Instance.Send(packet);
    }

    // 아이템 사용 가능 요청
    public void ItemUseRequest(uint index)
    {
        GamePacket packet = new GamePacket();
        packet.ItemUseRequest = new C2S_ItemUseRequest();

        packet.ItemUseRequest.InventorySlot = index + 1;

        GameServerSocketManager.Instance.Send(packet);
    }

    // 아이템 DisUse 요청
    public void ItemDisuseRequest(uint itemId)
    {
        GamePacket packet = new GamePacket();
        packet.ItemDisuseRequest = new C2S_ItemDisuseRequest();

        packet.ItemDisuseRequest.ItemId = itemId;

        GameServerSocketManager.Instance.Send(packet);
    }

    // 아이템 슬롯 변경 요청
    public void ItemChangeRequest(int index)
    {
        GamePacket packet = new GamePacket();
        packet.ItemChangeRequest = new C2S_ItemChangeRequest();

        packet.ItemChangeRequest.InventorySlot = (uint)index + 1;

        GameServerSocketManager.Instance.Send(packet);
    }

    // 아이템 버리기 요청
    public void ItemDiscardRequest(Item item, int index)
    {
        GamePacket packet = new GamePacket();
        packet.ItemDiscardRequest = new C2S_ItemDiscardRequest();

        ItemInfo itemInfo = new ItemInfo();
        Position position = new Position();

        Vector3 pos = GameManager.Instance.Player.ItemHoldHandler.transform.position;
        position.X = pos.x;
        position.Y = pos.y;
        position.Z = pos.z;

        itemInfo.ItemId = (uint)item.Id;
        itemInfo.ItemTypeId = (uint)item.ItemData.Type;
        itemInfo.Position = position;

        packet.ItemDiscardRequest.ItemInfo = itemInfo;
        packet.ItemDiscardRequest.InventorySlot = (uint)index + 1;

        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region State Request
    public void StateChangeRequest(CharacterState characterState)
    {
        GamePacket packet = new GamePacket();
        packet.PlayerStateChangeRequest = new C2S_PlayerStateChangeRequest();

        PlayerStateInfo playerStateInfo = new PlayerStateInfo();

        playerStateInfo.UserId = NetworkManager.Instance.UserId;
        playerStateInfo.CharacterState = characterState;

        packet.PlayerStateChangeRequest.PlayerStateInfo = playerStateInfo;
        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region Attacked (Hit) Request
    public void PlayerAttackedRequest(uint ghostId)
    {
        GamePacket packet = new GamePacket();
        packet.PlayerAttackedRequest = new C2S_PlayerAttackedRequest();

        packet.PlayerAttackedRequest.UserId = NetworkManager.Instance.UserId;
        packet.PlayerAttackedRequest.GhostId = ghostId;

        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region Life Update Request
    public void LifeUpdateRequest()
    {
        GamePacket packet = new GamePacket();
        packet.LifeUpdateRequest = new C2S_LifeUpdateRequest();
        
        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region Block Interaction Sync
    // Block Interaction Notification
    public void BlockInteraction()
    {
        GameManager.Instance.Player.InteractHandler.StopCheckRay(); // 코루틴 실행 방지
        GameManager.Instance.Player.InputHandler.PlayerInput.enabled = false;
    }

    // Start Stage Notification
    public void StartInteraction()
    {
        GameManager.Instance.Player.InteractHandler.StartCheckRay();
        GameManager.Instance.Player.InputHandler.PlayerInput.enabled = true;
    }

    #endregion

    #region State Change Sync
    public void StateChangeSync(CharacterState state)
    {
        switch(state)
        {
            case CharacterState.Exit:
                GameManager.Instance.CanRestartGame = true;
                break;
        }
    }

    #endregion

    #region Item Disuse Sync

    //  아이템 사용 중지 동기화 메서드
    public void ItemDisuseSync(Item item)
    {
        switch (item.ItemData.Type)
        {
            case EItemType.ITM0001:
                FlashLight flashLight = item as FlashLight;
                if (flashLight != null)
                    flashLight.TurnOff();
                break;
        }
    }
    #endregion
}