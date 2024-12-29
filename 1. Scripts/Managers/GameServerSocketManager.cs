using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
public class GameServerSocketManager : TCPSocketManagerBase<GameServerSocketManager>
{
    #region Gateway Property
    [Header("Gateway")]
    [field: SerializeField] public bool IsResponse { get; set; } = false;
    [field: SerializeField] public bool IsLoginSuccess { get; set; } = false;
    [field: SerializeField] public bool LoginInProgress { get; set; } = false;
    [field: SerializeField] public bool SignupSuccess { get; set; } = false;
    #endregion

    [Header("InGame")]
    public bool IsInStage = false;
    public bool IsRoomReady = false;
    public event Action OnMainSceneLoad;
    public event Action OnGameStart;
    private void Start()
    {
        ConnectGameServer();
    }
    public void ConnectGameServer()
    {
        IsInStage = false;
        ip = NetworkManager.Instance.GameServerIP;
        port = 6000;
        Init(ip, port);
        Connect();
    }
    public void CallMainSceneLoad()
    {
        OnMainSceneLoad?.Invoke();
    }
    private void OnApplicationQuit()
    {
        if (this == null) return;
        Disconnect();
    }
    // 아예 게임을 끄는 경우
    public override void Disconnect(bool isReconnect = false)
    {
        VivoxManager.Instance.VivoxDisconnect();
        VivoxManager.Instance.StopUpdate3DPosition();
        GameManager.Instance.Player.Disconnect();

        IsInStage = false;
        IsRoomReady = false;
        OnMainSceneLoad = null;
        OnGameStart = null;

        base.Disconnect(false);
    }
    public void EventInit()
    {
        OnMainSceneLoad = null;
        OnGameStart = null;
    }

    #region Gateway_Response
    public async void LoginResponse(GamePacket gamePacket)
    {
        if (!LoginInProgress) return;
        var response = gamePacket.LoginResponse;
        IsResponse = true;
        Debug.Log($"LoginfailCode : {response.GlobalFailCode}\nUserId : {response.UserId}\nToken : {response.Token}");
        if (response.GlobalFailCode == GlobalFailCode.AuthenticationFailed) IsLoginSuccess = false;
        else
        {
            IsLoginSuccess = true;
            NetworkManager.Instance.Token = response.Token;
            NetworkManager.Instance.UserId = response.UserId;
            NetworkManager.Instance.Nickname = response.Nickname;
            await VivoxManager.Instance.LoginAsync(NetworkManager.Instance.UserId);
            Debug.Log("LoginProcess : VivoxLogin...");
        }
        LoginInProgress = false;
    }
    public void RegistAccountResponse(GamePacket gamePacket)
    {
        var response = gamePacket.RegistAccountResponse;
        SignupSuccess = true;
    }
    public void ChangeNicknameResponse(GamePacket gamePacket) 
    {
        var response = gamePacket.ChangeNicknameResponse;
        NetworkManager.Instance.Nickname = response.Nickname;
        StartUIManager.Instance.ChangeNickname(response.Nickname);
    }
    public void EnterLobbyResponse(GamePacket gamePacket)
    {
        var response = gamePacket.EnterLobbyResponse;
        StartUIManager.Instance.OpenLobbyPage();
    }
    public void WaitingRoomListResponse(GamePacket gamePacket)
    {
        var response = gamePacket.WaitingRoomListResponse;
        StartUIManager.Instance.WaitingRoom.InitList();
        foreach (var room in response.RoomInfos)
        {
            // room 만들기
            // gameSessionId, roomName, numberOfPlayer, latency
            StartUIManager.Instance.WaitingRoom.CreateRoom(room.GameSessionId, room.RoomName, (int)room.NumberOfPlayer, (int)room.Latency);
        }
    }
    #endregion

    #region GameServer_S2C 1:1
    public void PingRequest(GamePacket gamePacket)
    {
        if (!isConnected) return;
        var response = gamePacket.PingRequest;
        var timestamp = response.Timestamp;
        GamePacket packet = new GamePacket();
        packet.PingResponse = new C2S_PingResponse();
        packet.PingResponse.Timestamp = timestamp;
        Send(packet);
    }
    public void CreateRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.CreateRoomResponse;
        NetworkManager.Instance.GameSessionId = response.GameSessionId;
        NetworkManager.Instance.InviteCode = response.InviteCode;
        NetworkManager.Instance.IsHost = true;
        IsRoomReady = true;
        /*
        UniTask task = LoadSceneManager.Instance.LoadMainScene(() =>
        {
            OnMainSceneLoad?.Invoke();
        });
        UIManager.Instance.UI_HUD.UI_RemainDay.InitDayText();    
        */
    }
    public void JoinRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomResponse;
        if(response.GlobalFailCode != GlobalFailCode.None)
        {
            Debug.Log("JoinRoomResponse 뭔가 연결에 문제있음");
            return;
        }
        NetworkManager.Instance.GameSessionId = response.GameSessionId;
        IsRoomReady = true;
        UniTask task = LoadMainSceneTask(response);
    }

    private UniTask LoadMainSceneTask(S2C_JoinRoomResponse response)
    {
        return LoadSceneManager.Instance.LoadMainScene(() =>
        {
            CallMainSceneLoad();
            UIManager.Instance.UI_HUD.UI_RemainDay.InitDayText();
            foreach (var user in response.PlayerInfos)
            {
                if (user.UserId == NetworkManager.Instance.UserId) continue;
                RemoteManager.Instance.CreatePlayer(user.UserId, user.Nickname);
            }
        });
    }

    public void DisconnectRoomResponse(GamePacket gamePacket)
    {
        
    }

    /// <summary>
    /// 미사용 SpawnInitialDataRequest
    /// </summary>
    /// <param name="gamePacket"></param>
    //public void SpawnInitialDataRequest(GamePacket gamePacket)
    //{
    //    var response = gamePacket.SpawnInitialDataRequest;
    //    if(response.GlobalFailCode != GlobalFailCode.None)
    //    {
    //        // 인증 실패 예외 처리
    //        return;
    //    }
    //    // stageId를 기반으로 ghost, item 제작
    //    GameManager.Instance.DiffId = response.DifficultyId;
    //    RemoteManager.Instance.InitStageData();
    //}
    public void PlayerLifeResponse(GamePacket gamePacket)
    {
        var response = gamePacket.PlayerLifeResponse;
        GameManager.Instance.Player.StatHandler.TakeDamage(response.Life, response.IsAttacked);
    }
    public void ItemGetResponse(GamePacket gamePacket)
    {
        var response = gamePacket.ItemGetResponse;
        Item item = RemoteManager.Instance.GetItem(response.ItemId);
        InventoryManager.Instance.Inventory.AddItem(item, (int)response.InventorySlot - 1);

    }
    public void ItemUseResponse(GamePacket gamePacket)
    {
        var response = gamePacket.ItemUseResponse;
        InventoryManager.Instance.Inventory.UseItem(response.InventorySlot - 1);
    }
    public void ItemDiscardResponse(GamePacket gamePacket)
    {
        var response = gamePacket.ItemDiscardResponse;
        InventoryManager.Instance.Inventory.ThrowItem((int)response.InventorySlot - 1);
    }
    public void ItemPurchaseResponse(GamePacket gamePacket)
    {
        var response = gamePacket.ItemPurchaseResponse;
        // 실패시 실패 UI 띄우기
        Debug.Log("아이템 구매 실패");
    }
    #endregion

    #region GameServer_S2C Noti
    #region Player
    public void PlayerMoveNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PlayerMoveNotification;
        foreach (var i in response.PlayerMoveInfos)
        {
            if (i.UserId == NetworkManager.Instance.UserId) continue;
            RemoteManager.Instance.UpdateRemotePlayerMovement(i.UserId, i.Position, i.Rotation);
        }
    }
    public void PlayerStateChangeNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PlayerStateChangeNotification;
        RemoteManager.Instance.UpdateRemotePlayerState(response.PlayerStateInfo.UserId, response.PlayerStateInfo.CharacterState);
    }
    public void BlockInteractionNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PlayerStateChangeNotification;
        RemoteManager.Instance.BlockInteractionPlayer();
    }
    #endregion

    #region Ghost
    public void GhostMoveNotification(GamePacket gamePacket)
    {
        if (NetworkManager.Instance.IsHost) return;
        var response = gamePacket.GhostMoveNotification;
        foreach (var i in response.GhostMoveInfos)
        {
            RemoteManager.Instance.UpdateRemoteGhostMovement(i.GhostId, i.Position, i.Rotation);
        }        
            
    }
    public void GhostStateChangeNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GhostStateChangeNotification;
        RemoteManager.Instance.UpdateGhostState(response.GhostStateInfo.GhostId, response.GhostStateInfo.CharacterState);
    }
    public void GhostSpawnNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GhostSpawnNotification;
        RemoteManager.Instance.CreateGhost(response.GhostInfo);
    }
    public void GhostDeleteNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GhostDeleteNotification;
        foreach(var ghostId in response.GhostIds)
            RemoteManager.Instance.DeleteGhost(ghostId);
    }
    public void GhostSpecialStateNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GhostSpecialStateNotification;
        uint id = response.GhostId;
        GhostSpecialState state = response.SpecialState;
        bool isOn = response.IsOn ? true : false;
        RemoteManager.Instance.UpdateGhostSpecialState(id, state, isOn);
    }
    #endregion

    #region Item
    public void ItemGetNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemGetNotification;
        RemoteManager.Instance.UpdateRemotePlayerGetItem(response.UserId, response.ItemId);
    }
    public void ItemChangeNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemChangeNotification;

        if(response.ItemId > 0)
        {
            RemoteManager.Instance.UpdateRemotePlayerChangeItem(response.UserId, response.ItemId);
        }
        else
        { 
            RemoteManager.Instance.UpdateRemotePlayerChangeItem(response.UserId);
        }
    }
    public void ItemUseNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemUseNotification;
        RemoteManager.Instance.UseItem(response.UserId, response.ItemId);
    }
    public void ItemDisuseNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemDisuseNotification;
        RemoteManager.Instance.DisuseItem(response.UserId, response.ItemId);
    }
    public void ItemDiscardNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemDiscardNotification;
        RemoteManager.Instance.DiscardItem(response.UserId, response.ItemId);
    }
    public void ItemDeleteNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemDeleteNotification;
        foreach (var itemId in response.ItemIds)
        {
            Debug.Log($"삭제되는 아이템 : {itemId}");
            RemoteManager.Instance.DeleteItem(itemId);
        } 
    }
    public void ItemCreateNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemCreateNotification;
        RemoteManager.Instance.CreateItem(response.ItemInfo);
    }
    public void ItemPurchaseNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ItemPurchaseNotification;
        RemoteManager.Instance.CreateItem(response.ItemInfo);
        UIManager.Instance.UI_HUD.UI_SoulCredit.UpdateCreditText((int)response.SoulCredit);
    }
    #endregion

    #region Door
    public void DoorToggleNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DoorToggleNotification;
        RemoteManager.Instance.UpdateRemoteDoorToggle(response.DoorId, response.DoorState);
    }
    #endregion

    #region System
    public void JoinRoomNotification(GamePacket gamePacket) 
    {
        var response = gamePacket.JoinRoomNotification;
        RemoteManager.Instance.CreatePlayer(response.UserId, response.Nickname);
    }
    public async UniTask WaitInstanceLoad(Action onComplete = null)
    {
        await UniTask.WaitUntil(() => RemoteManager.Instance != null);
        onComplete?.Invoke();
    }
    public void StartStageNotification(GamePacket gamePacket)
    {
        var response = gamePacket.StartStageNotification;
        IsInStage = true;

        foreach(var ghostInfo in response.GhostInfos)
        {
            RemoteManager.Instance.CreateGhost(ghostInfo);
        }

        foreach (var itemInfo in response.ItemInfos)
        {
            RemoteManager.Instance.CreateItem(itemInfo);
        }

        GameManager.Instance.Player.NetworkHandler.StartInteraction();
        OnGameStart?.Invoke();
        SoundManager.Instance.PlayBgm(EBgm.InGame);
    }
    /// <summary>
    /// 같이 플레이하던 친구가 disconnect 했을 경우
    /// -> RemoteManager에서 해당 Id에 해당하는 player 삭제
    /// </summary>
    /// <param name="gamePacket"></param>
    public void DisconnectPlayerNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DisconnectPlayerNotification;
        RemoteManager.Instance.DisconnectPlayer(response.UserId);
    }
    public void KickRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.KickRoomNotification;
        GameManager.Instance.Player.Disconnect();
        UniTask task = LoadSceneManager.Instance.LoadStartScene();
    }
    public void RemainingTimeNotification(GamePacket gamePacket)
    {
        var response = gamePacket.RemainingTimeNotification;
        UIManager.Instance.UI_HUD.UI_RemainTime.UpdateTimeText(response.RemainingTime);
    }
    public void StageEndNotification(GamePacket gamePacket)
    {
        var response = gamePacket.StageEndNotification;
        // current SoulCredit
        RemoteManager.Instance.isFailSesstion = false;
        IsInStage = false;
        GameManager.Instance.CallNextDay(response.StartPosition.ToVector3(), response.PenaltyCredit, response.RemainingDay, response.DiedCount, response.AliveCount);
        UIManager.Instance.UI_HUD.UI_SoulCredit.UpdateCreditText((int)response.SoulCredit);
    }
    public void SubmissionEndNotification(GamePacket gamePacket)
    {
        var response = gamePacket.SubmissionEndNotification;
        Debug.Log($"SubmissionEnd Result {response.Result}");
        if (!response.Result)
        {
            // 실패 시 실패 관련 연출 실행
            GameManager.Instance.CallFailEvent();
            UIManager.Instance.UI_HUD.UI_SoulCredit.UpdateTargetCreditText(200);
            UIManager.Instance.UI_HUD.UI_SoulCredit.InitCreditText();
            return;
        }

        // 성공 시 UI Update
        UIManager.Instance.OpenDDayUI();
        UIManager.Instance.UI_HUD.UI_SoulCredit.UpdateTargetCreditText((int)response.SubmissionValue);
        UIManager.Instance.UI_HUD.UI_RemainDay.InitDayText();
    }
    public void DifficultySelectNotification(GamePacket gamePacket)
    {
        GameManager.Instance.DiffSelector.gameObject.SetActive(false);
        UIManager.Instance.OpenHUDUI();
    }
    #endregion

    #region Extract
    public void ExtractSoulNotification(GamePacket gamePacket) 
    {
        var response = gamePacket.ExtractSoulNotification;
        UIManager.Instance.UI_HUD.UI_SoulCredit.UpdateCreditText((int)response.SoulCredit);
    }
    #endregion
    #endregion
}