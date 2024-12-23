using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public LocalPlayer Player;
    public DiffSelector DiffSelector;
    public event Action OnNextDay;
    public event Action OnFailSession;
    public uint DiffId = 1;
    public bool isStart = false;

    public bool CanRestartGame = false;
    public uint RemainingDay;
    public uint DiedPlayerNum;
    public uint AlivePlayerNum;

    private void Start()
    {
        isStart = false;
        Application.targetFrameRate = 144;
    }

    public async void CallNextDay(Vector3 pos, uint remainDay, uint diedPlayerNum, uint alivePlayerNum)
    {
        RemainingDay = remainDay;
        DiedPlayerNum = diedPlayerNum;
        AlivePlayerNum = alivePlayerNum;

        await UniTask.WaitUntil(() => (CanRestartGame));

        // Exit UI 
        UIManager.Instance.OpenExitUI();

        // UI
        UIManager.Instance.UI_HUD.UI_RemainDay.UpdateRemainDayText(RemainingDay);
        UIManager.Instance.UI_HUD.UI_RemainTime.InitTimeText();

        // System
        SoundManager.Instance.StopHeartBeatBgm();
        SoundManager.Instance.PlayBgm(EBgm.WaitingRoom);

        // Remote Player 
        RemoteManager.Instance.RevivalRemotePlayer(pos);

        // Local Player 
        Player.gameObject.SetActive(true);

        await VivoxManager.Instance.LeaveVoiceChannel(NetworkManager.Instance.InviteCode + "D"); // 죽었을 때 접속한 voice 채널 접속 해제
        await VivoxManager.Instance.Join3DChannel(Player.gameObject);
        //VivoxManager.Instance.Update3DChannelObj(Player.gameObject); // 다시 생성된 player obj 기준으로 vivox 서버 접속
        //VivoxManager.Instance.VoiceOnly3DChannel(); // mute 해제

        Player.PlayerInit();
        Player.Rigidbody.MovePosition(pos);

        OnNextDay?.Invoke();
        CanRestartGame = false;
        isStart = false;
    }

    public void CallFailEvent()
    {
        // 1. 문 잠기고, 2. 시야가 빨갛게 점등되고, 3. 모든 귀신들이 옆 문을 통해 들어온다.

        // FailBgm 실행
        SoundManager.Instance.PlayBgm(EBgm.Fail);

        OnFailSession?.Invoke();        // PlayerSight의 Light 이벤트 연결
    }
}
