using UnityEngine;

public class RemotePlayerNetworkHandler : MonoBehaviour
{
    private RemotePlayer player;
    private int prevAnimatorHash;
    private uint prevItemHoldIdx;

    public void Init(RemotePlayer player)
    {
        this.player = player;
    }

    #region Attacked Request
    public void RemotePlayerAttackedRequest(uint ghostId)
    {
        GamePacket packet = new GamePacket();
        packet.PlayerAttackedRequest = new C2S_PlayerAttackedRequest();

        packet.PlayerAttackedRequest.UserId = player.UserID;
        packet.PlayerAttackedRequest.GhostId = ghostId;

        GameServerSocketManager.Instance.Send(packet);
    }
    #endregion

    #region sync
    #region move sync
    // 이동 동기화 메서드
    public void MovementSync(Vector3 position, Vector3 rotation)
    {
        player.EventHandler.CallMoveEvent(position);
        player.EventHandler.CallLookEvent(rotation);
        //Debug.Log($"Rotation {rotation}");
    }
    #endregion

    #region item sync
    // 아이템 얻기 메서드
    public void ItemGetSync(Item item)
    {
        player.ItemHoldHandler.SetItemTransform(item, item.Id);
        item.RemotePickUp(player);
    }

    // 들고있는 아이템 동기화 메서드
    public void ItemBareHandChangeSync()
    {
        player.ItemHoldHandler.Barehand();
        player.AnimationHandler.DeActivateItemHoldLayer(player);
    }

    public void ItemChangeSync(Item item)
    {
        player.ItemHoldHandler.HoldItem(prevItemHoldIdx, false);
        player.ItemHoldHandler.HoldItem(item.Id, true);
        prevItemHoldIdx = item.Id;
        player.AnimationHandler.ActivateItemHoldLayer(player);
    }

    // 사용 아이템 동기화 메서드
    public void ItemUseSync(Item item)
    {
        switch (item.ItemData.Type)
        {
            case EItemType.ITM0001:
                FlashLight flashLight = item as FlashLight;
                if (flashLight != null)
                    flashLight.TurnOn();
                break;
        }
    }

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

    // 버리는 아이템 동기화 메서드
    public void ItemDiscardSync(Item item, Transform dropPos)
    {
        player.ItemHoldHandler.ThrowItem(item.Id, dropPos);
        item.RemoteThrow(player);
    }

    // 아이템 삭제 동기화 메서드
    public void ItemDeleteSync(Item item)
    {
        player.ItemHoldHandler.DeleteItem(item.Id);
        item.RemoteThrow(player);
    }
    #endregion

    #region state sync
    // 상태 동기화 메서드
    public void StateSync(CharacterState state)
    {
        // SetBool Anim - 실행 전 기존 애니메이션 Stop 후 진행 / Trigger - 상관 X
        switch(state)
        {
            case CharacterState.Idle:
                SetBoolAnimState(player.AnimationData.IdleParameterHash, player.AnimationData.IdleParameterHash);
                player.StopFootStepSound();
                break;
            case CharacterState.Move:
                SetBoolAnimState(player.AnimationData.MoveParameterHash, player.AnimationData.MoveParameterHash);
                player.StartFootStepSound(false);
                break;
            case CharacterState.Run:
                SetBoolAnimState(player.AnimationData.RunParameterHash, player.AnimationData.RunParameterHash);
                player.StartFootStepSound(true);
                break;
            case CharacterState.Jump:
                player.EventHandler.CallJumpEvent();
                player.AnimationHandler.StartAnimTrigger(player.AnimationData.JumpParameterHash, player);
                break;
            case CharacterState.Attack:
                player.AnimationHandler.StartAnimTrigger(player.AnimationData.AttackParameterHash, player);
                break;
            case CharacterState.Hit:
                player.AnimationHandler.StartAnimTrigger(player.AnimationData.HitParameterHash, player);
                break;
            case CharacterState.Died:
                player.EventHandler.CallDieEvent(player);
                player.AnimationHandler.StartAnimTrigger(player.AnimationData.DieParameterHash, player);
                break;
            case CharacterState.Exit:
                GameManager.Instance.CanRestartGame = true;
                break;
        }
    }

    private void SetBoolAnimState(int animParameterHash, int prevParameterHash)
    {
        player.AnimationHandler.StopAnimation(prevAnimatorHash, player);
        player.AnimationHandler.StartAnimation(animParameterHash, player);
        prevAnimatorHash = prevParameterHash;
    }
    #endregion

    #endregion
}
