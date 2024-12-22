using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button CreateRoomBtn;
    [SerializeField] Button JoinRoomBtn;
    [SerializeField] TMP_InputField InviteCode;

    private void Awake()
    {
        CreateRoomBtn.onClick.RemoveAllListeners();
        JoinRoomBtn.onClick.RemoveAllListeners();

        CreateRoomBtn.onClick.AddListener(CreateRoomRequest);
        JoinRoomBtn.onClick.AddListener(JoinRoomRequest);
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
        if (InviteCode.text.Length == 0) return;
        NetworkManager.Instance.InviteCode = InviteCode.text;
        GamePacket packet = new GamePacket();
        packet.JoinRoomRequest = new C2S_JoinRoomRequest();
        packet.JoinRoomRequest.UserId = NetworkManager.Instance.UserId;
        packet.JoinRoomRequest.Token = NetworkManager.Instance.Token;
        packet.JoinRoomRequest.InviteCode = InviteCode.text;
        Debug.Log($"{InviteCode.text} 게임 서버 연결");
        GameServerSocketManager.Instance.Send(packet);
    }
}
