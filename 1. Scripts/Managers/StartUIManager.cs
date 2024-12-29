using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;

public class StartUIManager : Singleton<StartUIManager>
{
    MainPanelManager mainPanelManager;
    [SerializeField] private string UI_HOME_STRING = "Home";
    [SerializeField] private string UI_SIGNUP_STRING = "Signup";
    [SerializeField] private string UI_LOGIN_STRING = "Login";
    [SerializeField] private string UI_MULTI_STRING = "Multiplayer";
    [SerializeField] private float duration = 1f;
    [field: SerializeField] public TMP_Text ProcessText { get; private set; }
    [field: SerializeField] public WaitingRoom WaitingRoom { get; private set; }
    [SerializeField] private Nickname UI_Nickname;
    protected override void Awake()
    {
        base.Awake();
        mainPanelManager = GetComponent<MainPanelManager>();
        ProcessText.gameObject.SetActive(false);
    }
    public void InitStartScene()
    {
        if (GameServerSocketManager.Instance.IsLoginSuccess)
            mainPanelManager.OpenPanel(UI_HOME_STRING);
        else
            mainPanelManager.OpenPanel(UI_LOGIN_STRING);
    }
    public void OpenLoginPage()
    {
        mainPanelManager.OpenPanel(UI_LOGIN_STRING);
    }
    public void OpenSignupPage()
    {
        mainPanelManager.OpenPanel(UI_SIGNUP_STRING);
    }
    public void OpenHomePage()
    {
        mainPanelManager.OpenPanel(UI_HOME_STRING);
    }
    public void OpenLobbyPage()
    {
        mainPanelManager.OpenPanel(UI_MULTI_STRING);
    }
    public void ChangeNickname(string nickname)
    {
        UI_Nickname.ChangeNickname(nickname);
    }
    public void EnterLobbyRequest()
    {
        ActiveProcessTextForSeconds("로비 접속중", 3f);
        GamePacket packet = new GamePacket();
        packet.EnterLobbyRequest = new C2S_EnterLobbyRequest();
        packet.EnterLobbyRequest.UserId = NetworkManager.Instance.UserId;
        GameServerSocketManager.Instance.Send(packet);
    }
    public void ActiveProcessText(string processText)
    {
        if(!ProcessText.gameObject.activeSelf)
            ProcessText.gameObject.SetActive(true);
        ProcessText.DOText($"{processText}...", duration);
    }
    public void DeActiveProcessText()
    {
        if (this == null) return;
        if (ProcessText.gameObject == null) return;
        if (!ProcessText.gameObject.activeSelf) return;
        ProcessText.text = "";
        ProcessText.gameObject.SetActive(false);
    }
    public async void ActiveProcessTextForSeconds(string processText, float time)
    {
        ActiveProcessText(processText);
        await UniTask.WaitForSeconds(time);
        DeActiveProcessText();
    }

    public void WaitingRoomRequest()
    {
        GamePacket packet = new GamePacket();
        packet.WaitingRoomListRequest = new C2S_WaitingRoomListRequest();
        packet.WaitingRoomListRequest.UserId = NetworkManager.Instance.UserId;
        GameServerSocketManager.Instance.Send(packet);
    }

    [ContextMenu("TEST")]
    public void Test()
    {
        mainPanelManager.OpenPanel(UI_HOME_STRING);
    }
}
