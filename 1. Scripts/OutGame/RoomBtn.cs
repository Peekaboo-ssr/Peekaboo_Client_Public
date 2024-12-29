using TMPro;
using UnityEngine;

public class RoomBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text gameSessionIdText;
    [SerializeField] private TMP_Text numberOfPlayerText;
    [SerializeField] private TMP_Text latencyText;
    string gameSessionId;
    public void Init(string gameSessionId, string roomName, int numberOfPlayer, int latency)
    {
        roomNameText.text = roomName;
        gameSessionIdText.text = gameSessionId;
        numberOfPlayerText.text = $"{numberOfPlayer} / 4";
        latencyText.text = $"{latency} MS";

        this.gameSessionId = gameSessionId;
    }

    public void JoinRoomByGameSessionIdRequest()
    {
        GamePacket packet = new GamePacket();
        packet.JoinRoomByGameSessionIdRequest = new C2S_JoinRoomByGameSessionIdRequest();
        packet.JoinRoomByGameSessionIdRequest.UserId = NetworkManager.Instance.UserId;
        packet.JoinRoomByGameSessionIdRequest.Token = NetworkManager.Instance.Token;
        packet.JoinRoomByGameSessionIdRequest.GameSessionId = gameSessionId;
        GameServerSocketManager.Instance.Send(packet);
    }
}
