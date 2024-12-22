using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_DISCONNECT : MonoBehaviour
{
    public void DisconnectRoomRequest()
    {
        GamePacket packet = new GamePacket();
        packet.DisconnectRoomRequest = new C2S_DisconnectRoomRequest();
        GameServerSocketManager.Instance.Send(packet);

        UniTask task = LoadSceneManager.Instance.LoadStartScene();
    }


}
