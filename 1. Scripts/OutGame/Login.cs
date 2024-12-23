using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using Michsky.UI.Dark;
using DG.Tweening;

public class Login : MonoBehaviour
{
    [SerializeField] private MainPanelManager mainPanelManager;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField pwInputField;
    [SerializeField] private Button loginBtn;
    [SerializeField] private TMP_Text loginBtnText;
    [SerializeField] private TMP_Text loginBtnTextHL;
    [SerializeField] private float duration = 1f;
    private string id, pw;
    
    private void Awake()
    {
        loginBtn.onClick.RemoveAllListeners();
        loginBtn.onClick.AddListener(login);
    }
    
    private async void login()
    {
        id = idInputField.text;
        pw = pwInputField.text;
        
        // LoginRequest 생성
        try
        {
            BlockLoginButton();
            await LoginRequest(id, pw).Timeout(TimeSpan.FromMilliseconds(30000));
            // Response가 왔을 때
            // TODO : 로그인 시도 UI Close
            Debug.Log("로그인 시도 중");
            // 로그인 성공
            if (GameServerSocketManager.Instance.IsLoginSuccess)
            {
                // TODO : LobbyPanel Open
                Debug.Log("로그인 성공");
                mainPanelManager.NextPage();
            }
            // 로그인 실패
            else
            {
                // TODO : 로그인 시도 UI Close, 로그인 실패 Log
                Debug.Log("로그인 실패(등록되지 않은 계정)");
                UnLockLoginButton();
            }
        }
        catch (TimeoutException)
        {
            // TODO : 로그인 시도 UI Close, 로그인 실패 Log
            Debug.LogError("시간 초과가 발생했습니다.");
            UnLockLoginButton();
        }
    }

    private async UniTask LoginRequest(string id, string pw)
    {
        // 텍스트 변경
        // ButtonComponent Off
        try
        {
            GameServerSocketManager.Instance.ConnectGameServer();
            await UniTask.WaitUntil(() => GameServerSocketManager.Instance.isConnected);
            Debug.Log("LoginProcess : 서버 연결...");
            await VivoxManager.Instance.LoginAsync(NetworkManager.Instance.UserId);
            Debug.Log("LoginProcess : VivoxLogin...");
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

    public void BlockLoginButton()
    {
        loginBtn.enabled = false;
        loginBtnText.DOText("LOGIN 시도 중...", duration);
        loginBtnTextHL.DOText("LOGIN 시도 중...", duration);
    }

    public void UnLockLoginButton()
    {
        loginBtn.enabled = true;
        loginBtnText.text = "LOGIN";
    }
}
