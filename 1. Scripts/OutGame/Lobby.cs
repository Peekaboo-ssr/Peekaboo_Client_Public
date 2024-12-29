using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button joinRoomBtn;
    [SerializeField] TMP_InputField inviteCode;

    private void Awake()
    {
        createRoomBtn.onClick.RemoveAllListeners();
        joinRoomBtn.onClick.RemoveAllListeners();

        createRoomBtn.onClick.AddListener(CreateRoomRequest);
        joinRoomBtn.onClick.AddListener(JoinRoomRequest);
    }

    private void CreateRoomRequest()
    {
        GamePacket packet = new GamePacket();
        packet.CreateRoomRequest = new C2S_CreateRoomRequest();
        packet.CreateRoomRequest.UserId = NetworkManager.Instance.UserId;
        packet.CreateRoomRequest.Token = NetworkManager.Instance.Token;
        GameServerSocketManager.Instance.Send(packet);

        UniTask task = LoadSceneManager.Instance.LoadMainScene(() =>
        {
            Debug.Log("게임 서버 연결");
            GameServerSocketManager.Instance.CallMainSceneLoad();
            UIManager.Instance.UI_HUD.UI_RemainDay.InitDayText();
        });
    }

    private void JoinRoomRequest()
    {
        if (inviteCode.text.Length == 0) return;
        NetworkManager.Instance.InviteCode = inviteCode.text;
        GamePacket packet = new GamePacket();
        packet.JoinRoomByInviteCodeRequest = new C2S_JoinRoomByInviteCodeRequest();
        packet.JoinRoomByInviteCodeRequest.UserId = NetworkManager.Instance.UserId;
        packet.JoinRoomByInviteCodeRequest.Token = NetworkManager.Instance.Token;
        packet.JoinRoomByInviteCodeRequest.InviteCode = inviteCode.text;
        Debug.Log($"{inviteCode.text} 게임 서버 연결");
        GameServerSocketManager.Instance.Send(packet);
    }
}
