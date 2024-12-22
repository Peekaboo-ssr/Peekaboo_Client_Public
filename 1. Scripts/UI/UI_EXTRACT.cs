using UnityEngine;

public class UI_EXTRACT : MonoBehaviour
{
    public void Open()
    {
        UIManager.Instance.OpenExtractUI();
    }

    public void ExtractSoulRequest()
    {
        GamePacket packet = new GamePacket();
        packet.ExtractSoulRequest = new C2S_ExtractSoulRequest();
        packet.ExtractSoulRequest.UserId = NetworkManager.Instance.UserId;
        GameServerSocketManager.Instance.Send(packet);
        UIManager.Instance.OpenHUDUI();
    }
}