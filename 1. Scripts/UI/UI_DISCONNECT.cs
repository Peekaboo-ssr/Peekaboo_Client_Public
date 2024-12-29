using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_DISCONNECT : MonoBehaviour
{
    public void DisconnectRoomRequest()
    {
        GamePacket packet = new GamePacket();
        packet.DisconnectRoomRequest = new C2S_DisconnectRoomRequest();
        GameServerSocketManager.Instance.Send(packet);
        GameServerSocketManager.Instance.IsInStage = false;
        GameServerSocketManager.Instance.IsRoomReady = false;
        GameServerSocketManager.Instance.EventInit();

        VivoxManager.Instance.StopUpdate3DPosition();
        GameManager.Instance.Player.Disconnect();

        UniTask task = LoadSceneManager.Instance.LoadStartScene();
    }
}
