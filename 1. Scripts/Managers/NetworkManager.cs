using Cysharp.Threading.Tasks;
using UnityEngine;

// 통합 네트워크 매니저 -> GatewayManager + GameServerManager로 분리
public class NetworkManager : Singleton<NetworkManager>
{
    [field: Header("User 정보")]
    [field: SerializeField] public string UserId { get; set; }
    [field: SerializeField] public string Nickname { get; set; }
    [field: SerializeField] public string Token { get; set; }

    [field: Header("Network 정보")]
    [field: SerializeField] public string GameServerIP { get; private set; } = "127.0.0.1";

    [Header("세션 정보")]
    public string InviteCode;
    public string GameSessionId;
    public bool IsHost = false;

    private void Start()
    {
        IsHost = false;
    }

    #region Server통신
    // TODO : UI 제작 및 게임 시작 방식 연동 필요
    [ContextMenu("StartStageReq")]
    public async void StartStageRequest()
    {
        GamePacket packet = new GamePacket();
        packet.StartStageRequest = new C2S_StartStageRequest();
        packet.StartStageRequest.GameSessionId = GameSessionId;
        GameServerSocketManager.Instance.Send(packet);

        await UniTask.WaitUntil(() => GameServerSocketManager.Instance.IsInStage);
        Debug.Log("Ready");
    }

    [ContextMenu("CreateItemReq")]
    public void CreateItemRequest()
    {
        GamePacket packet = new GamePacket();
        packet.ItemCreateRequest = new C2S_ItemCreateRequest();
        packet.ItemCreateRequest.ItemTypeId = (uint)EItemType.ITM0001;
        GameServerSocketManager.Instance.Send(packet);
    }

    public void InitNetworkManager()
    {
        InviteCode = null;
        GameSessionId = null;
        IsHost = false;
    }
    #endregion
}