using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

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

    private void OnParticipantAddedToChannel(VivoxParticipant participant)
    {
        participants[participant.PlayerId] = new Tuple<VivoxParticipant, Player>(participant, GameManager.Instance.Player);
    }

    private void OnParticipantRemovedFromChannel(VivoxParticipant participant)
    {
        if (participants.ContainsKey(participant.PlayerId))
        {
            participants.Remove(participant.PlayerId);
        }
    }

    // Vivox 로그인
    public async UniTask LoginAsync(string userId)
    {
        // 로그인 옵션 생성
        LoginOptions options = new LoginOptions();

        // 고유 PlayerId 지정
        options.PlayerId = userId;

        // 디스플레이 이름 설정 
        options.DisplayName = userId;

        //로그인 진행
        await VivoxService.Instance.LoginAsync(options);
    }

    // 일반 채널 접속 - 죽었을 때
    public async UniTask JoinVoiceChannel(string sessionId)
    {
        // 접속되어 있지 않은 채널인 경우에만 Join
        if (!VivoxService.Instance.ActiveChannels.ContainsKey(sessionId))
            //음성채팅 채널에 접속 / channelName -> Session Id
            await VivoxService.Instance.JoinGroupChannelAsync(sessionId, ChatCapability.AudioOnly);
    }

    // 3D 음성 채널 접속 - 인게임
    public async UniTask Join3DChannel(GameObject speakObj)
    {
        // 접속되어 있지 않은 채널인 경우에만 Join
        if (!VivoxService.Instance.ActiveChannels.ContainsKey(NetworkManager.Instance.InviteCode))
            //위치 음성 채널에 접속
            await VivoxService.Instance.JoinPositionalChannelAsync(NetworkManager.Instance.InviteCode, ChatCapability.AudioOnly, channel3DSetting.GetChannel3DProperties());

        //위치를 주기적으로 업데이트하는 코루틴 실행
        if (update3DPositionCoroutine != null)
        {
            StopCoroutine(update3DPositionCoroutine);
        }
        update3DPositionCoroutine = StartCoroutine(Update3DPositionCo(speakObj));
    }

    private IEnumerator Update3DPositionCo(GameObject speakObj)
    {
        while (true)
        {
            //위치 업데이트
            VivoxService.Instance.Set3DPosition(speakObj, NetworkManager.Instance.InviteCode);
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

    public async UniTask LeaveVoiceChannel(string sessionId)
    {
        // 접속되어 있는 채널인 경우에만 leave
        if(VivoxService.Instance.ActiveChannels.ContainsKey(sessionId))
            await VivoxService.Instance.LeaveChannelAsync(sessionId);
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
        VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, NetworkManager.Instance.InviteCode + "D");
    }

    public void VoiceOnly3DChannel()
    {
        VivoxService.Instance.SetChannelTransmissionModeAsync(TransmissionMode.Single, NetworkManager.Instance.InviteCode);
    }
}