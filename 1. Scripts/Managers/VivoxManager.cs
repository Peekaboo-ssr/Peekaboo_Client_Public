using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Channel3DSetting
{
    //가청거리 - 소리가 들리는 최대 거리
    [SerializeField] private int audibleDistance = 10;

    //conversationalDistance 이내에서는 감쇠 없이 최대 음량으로 들림
    [SerializeField] private int conversationalDistance = 1;

    //FadeModel에 따른 감쇠 강도 - 거리 기반으로 음량 감쇠를 얼마나 강하게 적용할지 여부
    [SerializeField] private float audioFadeIntensityByDistance = 1.0f;

    //위치에 따른 음량 감쇠 모델 - 거리가 멀어질수록 음량이 비례적으로 작아지는 모델 선정
    [SerializeField] private AudioFadeModel audioFadeModel = AudioFadeModel.InverseByDistance;

    // 위의 속성값을 기반으로 3D 설정값 객체를 생성
    public Channel3DProperties GetChannel3DProperties()
    {
        return new Channel3DProperties(audibleDistance, conversationalDistance, audioFadeIntensityByDistance, audioFadeModel);
    }
}

public class VivoxManager : Singleton<VivoxManager>
{
    public Channel3DSetting channel3DSetting;

    private Coroutine update3DPositionCoroutine;
    private readonly WaitForSeconds positonUpdateRate = new WaitForSeconds(0.5f);

    public Dictionary<string, Tuple<VivoxParticipant,Player>> participants = new Dictionary<string, Tuple<VivoxParticipant, Player>>();

    private async void Start()
    {
        //유니티 서비스 초기화
        await UnityServices.InitializeAsync();

        //AuthenticationService를 사용하여 익명 인증
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //Vivox 초기화
        await VivoxService.Instance.InitializeAsync(); 

        VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAddedToChannel;
        VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemovedFromChannel;
}

    #region Participants
    private async void OnParticipantAddedToChannel(VivoxParticipant participant)
    {
        string channelId = participant.ChannelName;
        if (channelId != NetworkManager.Instance.GameSessionId) return; // 3D 공간 서버가 아닌 죽은자들의 서버에 Add된 경우 return

        if (participant.IsSelf)
        {
            participants[participant.PlayerId] = new Tuple<VivoxParticipant, Player>(participant, GameManager.Instance.Player);
            participants[participant.PlayerId].Item1.ParticipantAudioEnergyChanged += () => UIManager.Instance.UI_HUD.UI_Noise.SetNoise((float)participants[participant.PlayerId].Item1.AudioEnergy);
        }
        else
        {
            await UniTask.WaitUntil(() => RemoteManager.Instance.PlayerDictionary.ContainsKey(participant.DisplayName)); // PlayerDictionary에 Player가 등록될 때까지 대기
            participants[participant.PlayerId] = new Tuple<VivoxParticipant, Player>(participant, RemoteManager.Instance.PlayerDictionary[participant.DisplayName]);
        }        
    }

    private void OnParticipantRemovedFromChannel(VivoxParticipant participant)
    {
        string channelId = participant.ChannelName;
        if (channelId != NetworkManager.Instance.GameSessionId) return; // 3D 공간 서버가 아닌 죽은자들의 서버에서 Remove된 경우 return

        if (participants.ContainsKey(participant.PlayerId))
        {
            participants.Remove(participant.PlayerId);
        }
    }
    #endregion

    #region Login
    // Vivox 로그인
    public async UniTask LoginAsync(string userId)
    {
        // 로그인 옵션 생성
        LoginOptions options = new LoginOptions();

        // 고유 PlayerId 지정
        options.PlayerId = userId;

        // 디스플레이 이름 UserID로 수정
        options.DisplayName = userId;

        //로그인 진행
        await VivoxService.Instance.LoginAsync(options);
    }
    #endregion

    #region Died Channel
    // 일반 채널 접속 - 죽었을 때
    public async UniTask JoinVoiceChannel(string sessionId)
    {
        // 접속되어 있지 않은 채널인 경우에만 Join
        if (!VivoxService.Instance.ActiveChannels.ContainsKey(sessionId))
            //음성채팅 채널에 접속 / channelName -> Session Id
            await VivoxService.Instance.JoinGroupChannelAsync(sessionId, ChatCapability.AudioOnly);
    }

    public async UniTask LeaveVoiceChannel(string sessionId)
    {
        // 접속되어 있는 채널인 경우에만 leave
        if (VivoxService.Instance.ActiveChannels.ContainsKey(sessionId))
            await VivoxService.Instance.LeaveChannelAsync(sessionId);
    }

    #endregion

    #region 3D Channel
    // 3D 음성 채널 접속 - 인게임
    public async UniTask Join3DChannel(GameObject speakObj)
    {
        // 접속되어 있지 않은 채널인 경우에만 Join
        if (!VivoxService.Instance.ActiveChannels.ContainsKey(NetworkManager.Instance.GameSessionId))
            //위치 음성 채널에 접속
            await VivoxService.Instance.JoinPositionalChannelAsync(NetworkManager.Instance.GameSessionId, ChatCapability.AudioOnly, channel3DSetting.GetChannel3DProperties());

        StartUpdate3DPosition(speakObj);
    }

    public void StopUpdate3DPosition()
    {
        if (update3DPositionCoroutine != null)
            StopCoroutine(update3DPositionCoroutine);
    }

    public void StartUpdate3DPosition(GameObject speakObj)
    {
        StopUpdate3DPosition();
        update3DPositionCoroutine = StartCoroutine(Update3DPositionCo(speakObj));
    }

    private IEnumerator Update3DPositionCo(GameObject speakObj)
    {
        while (true)
        {
            if (NetworkManager.Instance == null || this == null || NetworkManager.Instance.GameSessionId == null || speakObj == null)
                yield break;
            VivoxService.Instance.Set3DPosition(speakObj, NetworkManager.Instance.GameSessionId);
            yield return positonUpdateRate;
        }
    }

    public void Update3DChannelObj(GameObject speakObj)
    {
        if (update3DPositionCoroutine != null)
        {
            StopCoroutine(update3DPositionCoroutine);
        }
        update3DPositionCoroutine = StartCoroutine(Update3DPositionCo(speakObj));
    }

    public async UniTask Leave3DVoiceChannel(string sessionId)
    {
        // 접속되어 있는 채널인 경우에만 leave
        if (VivoxService.Instance.ActiveChannels.ContainsKey(sessionId))
        {
            //위치를 주기적으로 업데이트하는 코루틴 실행
            if (update3DPositionCoroutine != null)
            {
                StopCoroutine(update3DPositionCoroutine);
            }
            await VivoxService.Instance.LeaveChannelAsync(sessionId);
        }
    }

    #endregion

    #region Voice Property
    public void LeaveAllChannel()
    {
        VivoxService.Instance.LeaveAllChannelsAsync();
    }

    public async void VivoxDisconnect()
    {
        await VivoxService.Instance.LogoutAsync();
    }

    public void VoiceOnlyDieChannel()
    {
        try
        {
             VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, NetworkManager.Instance.GameSessionId + "D");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}. Die 단일 채널에 Voice 전송이 이루어지지 않습니다.");
        }
    }

    public void VoiceOnly3DChannel()
    {
        try
        {
             VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, NetworkManager.Instance.GameSessionId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}. 3D 단일 채널에 Voice 전송이 이루어지지 않습니다.");
        }
    }

    public void VoiceMute()
    {
        VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.None);
    }
    #endregion 
}