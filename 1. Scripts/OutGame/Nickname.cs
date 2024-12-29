using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private Button nicknameChangeReqBtn;
    [SerializeField] private TMP_Text nicknamePlaceHolder;

    private async void Start()
    {
        await UniTask.WaitUntil(() => NetworkManager.Instance.Nickname.Length != 0);
        nicknamePlaceHolder.text = NetworkManager.Instance.Nickname;
        nicknameChangeReqBtn.onClick.RemoveAllListeners();
        nicknameChangeReqBtn.onClick.AddListener(ChangeNicknameRequest);
    }

    private void ChangeNicknameRequest()
    {
        if (nicknameField.text.Length == 0) return;
        GamePacket packet = new GamePacket();
        packet.ChangeNicknameRequest = new C2S_ChangeNicknameRequest();
        packet.ChangeNicknameRequest.UserId = NetworkManager.Instance.UserId;
        packet.ChangeNicknameRequest.Nickname = nicknameField.text;
        GameServerSocketManager.Instance.Send(packet);
    }

    public void ChangeNickname(string nickname)
    {
        nicknamePlaceHolder.text = nickname;
    }
}