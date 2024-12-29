using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Signup : MonoBehaviour
{
    [SerializeField] private TMP_InputField id;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField nickname;
    [SerializeField] private Button signupBtn;
    private Login loginPanel;

    private EventSystem system;
    [SerializeField] private Selectable firstInput;

    private void Start()
    {
        system = EventSystem.current;
        firstInput.Select();

        signupBtn.onClick.RemoveAllListeners();
        signupBtn.onClick.AddListener(SignupRequest);
    }

    public void Init(Login panel)
    {
        loginPanel = panel;
        id.text = "";
        password.text = "";
        nickname.text = "";
    }

    public async void SignupRequest()
    {
        if (!GameServerSocketManager.Instance.isConnected)
        {
            GameServerSocketManager.Instance.ConnectGameServer();
            await UniTask.WaitUntil(() => GameServerSocketManager.Instance.isConnected);
        }
        if(id.text.Length == 0)
        {
            IDFail();
            return;
        }
        if(nickname.text.Length > 10)
        {
            NicknameFail();
            return;
        }
        GamePacket packet = new GamePacket();
        packet.RegistAccountRequest = new C2S_RegistAccountRequest();
        packet.RegistAccountRequest.Id = id.text;
        packet.RegistAccountRequest.Password = password.text;
        packet.RegistAccountRequest.Nickname = nickname.text;
        GameServerSocketManager.Instance.Send(packet);
        loginPanel.ReturnToLoginPage();
    }

    public void NextTab()
    {
        Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
        if (next != null)
        {
            next.Select();
        }
    }

    private void IDFail()
    {
        // 기능 추가
        Debug.LogError("ID를 입력해주세요 (눈에 보이는 뭔가 제작 필요)");
    }

    private void NicknameFail()
    {
        Debug.LogError("닉네임은 10글자 이내로 작성해주세요 (눈에 보이는 뭔가 제작 필요)");
    }
}