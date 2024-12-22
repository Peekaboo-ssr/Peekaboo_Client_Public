using System;
using UnityEngine;

[Serializable]
public class SoundData
{
    public float MasterVolume;
    public float BgmVolume;
    public float BgmHeartBeatVolume;
    public float SfxInGameVolume;

    public SoundData(float masterVolume, float bgmVolume, float bgmHeartBeatVolume, float sfxInGameVolume)
    {
        MasterVolume = masterVolume;
        BgmVolume = bgmVolume;
        BgmHeartBeatVolume = bgmHeartBeatVolume;
        SfxInGameVolume = sfxInGameVolume;
    }

    // 사운드 데이터 저장할 때 사용
    public void SetSoundVolume()
    {
        MasterVolume = SoundManager.Instance.GetAudioMixerVolume(EAudioMixerType.Master);
        BgmVolume = SoundManager.Instance.GetAudioMixerVolume(EAudioMixerType.Bgm);
        BgmHeartBeatVolume = SoundManager.Instance.GetAudioMixerVolume(EAudioMixerType.BgmHeartBeat);
        SfxInGameVolume = SoundManager.Instance.GetAudioMixerVolume(EAudioMixerType.SfxInGame);
    }
}
