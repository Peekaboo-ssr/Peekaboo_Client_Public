using UnityEngine;
using System.Linq;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

[Serializable]
public class AudioInfo<T> where T : Enum
{
    public T EAudioType;
    public AudioClip AudioClip;
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("# Audio Mixer ")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmAudioMixer;
    [SerializeField] private AudioMixerGroup heartBeatBgmAudioMixer;
    [SerializeField] private AudioMixerGroup sfxInGameAudioMixer;

    [Header("# Bgm Info")]
    [SerializeField] private AudioInfo<EBgm>[] bgmClips; // 전반적인 게임 Bgm
    [SerializeField] private float bgmVolume;
    private AudioSource bgmPlayer;
    private Dictionary<int, AudioClip> bgmDictionary = new Dictionary<int, AudioClip>();

    [Header("# Player Health Bgm Info")]
    [SerializeField] private AudioInfo<EHeartBeatBgm>[] heartBeatBgmClips; // Player의 심장소리
    [SerializeField] private float heartBeatBgmVolume;
    private AudioSource heartBeatBgmPlayer;
    private Dictionary<int, AudioClip> heartBeatBgmDictionary = new Dictionary<int, AudioClip>();

    [Header("# SFX InGame Info")] // 인게임 Sfx
    [SerializeField] private AudioInfo<EInGameSfx>[] sfxInGameClips;
    [SerializeField] private float sfxInGameVolume;
    private AudioSource[] sfxInGamePlayers;
    private Dictionary<int, AudioClip> sfxInGameDictionary = new Dictionary<int, AudioClip>();

    [Header("# Sound Info")]
    [SerializeField] private int channels; // 많은 효과음을 내기 위한 채널 시스템
    [SerializeField][Range(0f, 1f)] private float soundEffectPitchVariance; // 피치가 높아지면 높은 소리가 남

    private int channelIndex; // 채널 갯수 만큼 순회하도록 맨 마지막에 플레이 했던 SFX의 인덱스번호를 저장하는 변수

    // AudioMixer 개수대로 초기화
    private bool[] isMute = new bool[3];
    private float[] audioVolumes = new float[3];

    private void Start()
    {
        Init();
    }

    #region Init
    private void Init()
    {
        // BgmPlayer 초기화
        InitBgmPlayer("BgmPlayer", bgmVolume, bgmAudioMixer, ref bgmPlayer);
        InitDicionary<EBgm>(bgmClips, bgmDictionary);

        //PlayerHealthBgmPlayer 초기화
        InitBgmPlayer("HeartBeatBgmPlayer", heartBeatBgmVolume, heartBeatBgmAudioMixer,ref  heartBeatBgmPlayer);
        InitDicionary<EHeartBeatBgm>(heartBeatBgmClips, heartBeatBgmDictionary);

        // SFXInGamePlayer 초기화
        sfxInGamePlayers = InitSfxPlayer("SFXInGamePlayer", sfxInGameVolume, sfxInGameAudioMixer);
        InitDicionary<EInGameSfx>(sfxInGameClips, sfxInGameDictionary);
    }

    private void InitBgmPlayer(string objName, float volume, AudioMixerGroup bgmAudioMixer, ref AudioSource audioPlayer)
    {
        GameObject bgmObject = new GameObject(objName);
        bgmObject.transform.parent = transform;

        audioPlayer = bgmObject.AddComponent<AudioSource>();
        audioPlayer.loop = true; // 반복 true
        audioPlayer.volume = volume; // 볼륨 
        audioPlayer.outputAudioMixerGroup = bgmAudioMixer; // bgmAudioMixer를 통해 출력
    }

    private AudioSource[] InitSfxPlayer(string objName, float volume, AudioMixerGroup sfxAudioMixer)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.parent = transform;

        // 채널 개수대로 초기화
        AudioSource[] players = new AudioSource[channels];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = obj.AddComponent<AudioSource>();
            players[i].playOnAwake = false; // Play 메서드를 통해서만 실행
            players[i].volume = volume;
            players[i].bypassListenerEffects = true; // 하이패스에 안 걸리게 함
            players[i].outputAudioMixerGroup = sfxAudioMixer; // sfxAudioMixer 통해 출력
            players[i].spatialBlend = 1; // 3D 사운드로 설정
        }
        return players;
    }

    private void InitDicionary<T>(AudioInfo<T>[] audioClips, Dictionary<int, AudioClip> dictionary) where T : Enum
    {
        foreach (var clips in audioClips)
        {
            int key = Convert.ToInt32(clips.EAudioType);
            dictionary[key] = clips.AudioClip;
        }
    }
    #endregion

    #region Play 
    public void PlayBgm(EBgm eBgm) // Bgm 플레이 함수
    {
        if (!bgmPlayer.loop) // BgmPlayer의 반복이 켜져있다면 false
        {
            bgmPlayer.loop = true;
        }
        bgmPlayer.clip = bgmDictionary[(int)eBgm];
        bgmPlayer.Play(); // Bgm 플레이
    }

    public void PlayHeartBeatBgm(EHeartBeatBgm eHeartBeatBgm) 
    {
        if (!heartBeatBgmPlayer.loop) 
        {
            heartBeatBgmPlayer.loop = true;
        }
        heartBeatBgmPlayer.clip = heartBeatBgmDictionary[(int)eHeartBeatBgm];
        heartBeatBgmPlayer.Play(); 
    }

    public void PlayInGameSfx(EInGameSfx eSfx, Vector3 soundPosition)
    {
        for (int i = 0; i < sfxInGamePlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxInGamePlayers.Length;
            if (sfxInGamePlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex; // 현재 실행할 index 저장
            sfxInGamePlayers[loopIndex].transform.position = soundPosition;
            sfxInGamePlayers[loopIndex].clip = sfxInGameDictionary[(int)eSfx];
            SetRandomPitchToSfx(loopIndex, sfxInGamePlayers);
            sfxInGamePlayers[loopIndex].Play();
            break;
        }
    }
    #endregion

    #region Stop
    public void StopBgm()
    {
        bgmPlayer.Stop();
    }
    public void StopHeartBeatBgm()
    {
        heartBeatBgmPlayer.Stop();
    }
    #endregion

    #region Audio Processing 
    // 랜덤 Pitch 적용하여 같은 소리라도 단조롭게 들리지 않도록 함
    private void SetRandomPitchToSfx(int loopIndex, AudioSource[] sfxPlayers)
    {
        sfxPlayers[loopIndex].pitch = 1f + UnityEngine.Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);
    }

    // EAudioMixerType 타입의 오디오를 Mute
    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (!isMute[type]) // Mute 되어있지 않을 때
        {
            isMute[type] = true;
            audioVolumes[type] = GetAudioMixerVolume(audioMixerType);   // 현재 볼륨 저장해두기
            SetAudioVolume(audioMixerType, 0.0001f);
        }
    }

    // EAudioMixerType 타입의 오디오를 Mute 해제
    public void AudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (isMute[type]) // 이미 Mute 되어있으면
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }
    #endregion

    #region Setting
    // 저장할 float 값 뽑기
    public float GetAudioMixerVolume(EAudioMixerType audioMixerType)
    {
        audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
        return curVolume;
    }

    // float -> volume으로 변환하여 설정
    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        // volume은 0.0001~1 값이 들어오도록 방어코드 작성
        if (volume < 0.0001f)
            volume = 0.0001f;
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20f);
    }

    // 현재 오디오 볼륨을 0~1사이 값으로 변환
    public float GetAudioVolume(EAudioMixerType audioMixerType)
    {
        // volumeDb는 -80~0 사이값 가짐
        audioMixer.GetFloat(audioMixerType.ToString(), out float volumeDb);
        // 10의 volumeDb/20 거듭제곱
        float linearVolume = Mathf.Pow(10, volumeDb / 20f);
        // linearVolume 값을 0과  1사이의 값으로 변한
        return Mathf.Clamp01(linearVolume);
    }

    // 사운드 데이터 적용할 때 사용
    public void SetAudioMixerVolume(SoundData data)
    {
        SetAudioVolume(EAudioMixerType.Master, Mathf.Pow(10, data.MasterVolume / 20f));
        SetAudioVolume(EAudioMixerType.Bgm, Mathf.Pow(10, data.BgmVolume / 20f));
        SetAudioVolume(EAudioMixerType.BgmHeartBeat, Mathf.Pow(10, data.BgmHeartBeatVolume / 20f));
        SetAudioVolume(EAudioMixerType.SfxInGame, Mathf.Pow(10, data.SfxInGameVolume / 20f));
    }
    #endregion
}