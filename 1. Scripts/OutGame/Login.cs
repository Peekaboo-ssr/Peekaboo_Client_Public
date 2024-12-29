using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using Michsky.UI.Dark;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Login : MonoBehaviour
{
    [SerializeField] private MainPanelManager mainPanelManager;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField pwInputField;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button signupBtn;
    
    private string id, pw;

    public Signup SignupPanel;
    private EventSystem system;

    [SerializeField] private float duration = 1f;
    [SerializeField] private Selectable firstInput;

    private void Awake()
    {
        loginBtn.onClick.RemoveAllListeners();
        loginBtn.onClick.AddListener(login);
        signupBtn.onClick.RemoveAllListeners();
        signupBtn.onClick.AddListener(SignupPanelOpen);
    }
    private void Start()
    {
        system = EventSystem.current;
        firstInput.Select();
    }
    #region Login
    public async void login()
    {
        id = idInputField.text;
        pw = pwInputField.text;
        
        // LoginRequest 생성
        try
        {
            LoginProcess();
            await LoginRequest(id, pw).Timeout(TimeSpan.FromMilliseconds(30000));
            // Response가 왔을 때
            // TODO : 로그인 시도 UI Close
            Debug.Log("로그인 시도 중");
            // 로그인 성공
            if (GameServerSocketManager.Instance.IsLoginSuccess)
            {
                // TODO : LobbyPanel Open
                Debug.Log("로그인 성공");
                StartUIManager.Instance.OpenHomePage();
                DoneLoginProcess();
            }
            // 로그인 실패
            else
            {
                // TODO : 로그인 시도 UI Close, 로그인 실패 Log
                StartUIManager.Instance.ActiveProcessTextForSeconds("로그인 실패(등록되지 않은 계정)", 1f);
                Debug.Log("로그인 실패(등록되지 않은 계정)");
                UnLockBtn();
            }
        }
        catch (TimeoutException)
        {
            // TODO : 로그인 시도 UI Close, 로그인 실패 Log
            StartUIManager.Instance.ActiveProcessTextForSeconds("로그인 실패(등록되지 않은 계정)", 1f);
            Debug.Log("시간 초과가 발생했습니다.");
            UnLockBtn();
        }
    }
    private async UniTask LoginRequest(string id, string pw)
    {
        // 텍스트 변경
        // ButtonComponent Off
        try
        {
            if (!GameServerSocketManager.Instance.isConnected)
            {
                GameServerSocketManager.Instance.ConnectGameServer();
                await UniTask.WaitUntil(() => GameServerSocketManager.Instance.isConnected);
                Debug.Log("LoginProcess : 서버 연결...");
            }

            GameServerSocketManager.Instance.LoginInProgress = true;
            GamePacket packet = new GamePacket();
            packet.LoginRequest = new C2S_LoginRequest();
            packet.LoginRequest.Id = id;
            packet.LoginRequest.Password = pw;
            GameServerSocketManager.Instance.Send(packet);

            // TODO : 로그인 시도 중 팝업 Msg 출력
            // 로그인 시도 중일 때만 Response의 값을 체크
            await UniTask.WaitUntil(() => GameServerSocketManager.Instance.IsResponse == true);
            GameServerSocketManager.Instance.IsResponse = false;
        }
        catch(Exception ex) 
        {
            Debug.Log(ex);
        }
    }
    public void LoginProcess()
    {
        BlockBtn();
        StartUIManager.Instance.ActiveProcessText("LOGIN 시도 중...");
    }
    public void DoneLoginProcess()
    {
        UnLockBtn();
        StartUIManager.Instance.DeActiveProcessText();
    }
    #endregion
    private void SignupPanelOpen()
    {
        StartUIManager.Instance.OpenSignupPage();
        SignupPanel.Init(this);
    }
    public async void ReturnToLoginPage()
    {
        StartUIManager.Instance.OpenLoginPage();
        StartUIManager.Instance.ActiveProcessText("등록중");
        await UniTask.WaitUntil(() => GameServerSocketManager.Instance.SignupSuccess);
        StartUIManager.Instance.ActiveProcessText("등록완료");
        await UniTask.WaitForSeconds(1f);
        StartUIManager.Instance.DeActiveProcessText();
        // 실패시 실패 Log 띄워주기
    }
    private void BlockBtn()
    {
        loginBtn.enabled = false;
        signupBtn.enabled = false;
    }
    private void UnLockBtn()
    {
        loginBtn.enabled = true;
        signupBtn.enabled = true;
    }
    public void NextTab()
    {
        Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
        if (next != null)
        {
            next.Select();
        }
    }
}
