using UnityEngine;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    [SerializeField] private Slider MasterVolume;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider SFXVolume;

    private void Start()
    {
        SettingManager.Instance.AudioSetting = this;
        Init();
    }

    private void Init()
    {
        MasterVolume.value = SoundManager.Instance.GetAudioVolume(EAudioMixerType.Master);
        MasterVolume.onValueChanged.RemoveAllListeners();
        MasterVolume.onValueChanged.AddListener(SetMasterVolumeChange);

        MusicVolume.value = SoundManager.Instance.GetAudioVolume(EAudioMixerType.Bgm);
        MusicVolume.onValueChanged.RemoveAllListeners();
        MusicVolume.onValueChanged.AddListener(SetMusicVolumeChange);


        SFXVolume.value = SoundManager.Instance.GetAudioVolume(EAudioMixerType.SfxInGame);
        SFXVolume.onValueChanged.RemoveAllListeners();
        SFXVolume.onValueChanged.AddListener(SetSFXVolumeChange);

    }

    private void SetMasterVolumeChange(float value)
    {
        SoundManager.Instance.SetAudioVolume(EAudioMixerType.Master, value);
    }

    private void SetMusicVolumeChange(float value)
    {
        SoundManager.Instance.SetAudioVolume(EAudioMixerType.Bgm, value);
    }

    private void SetSFXVolumeChange(float value)
    {
        SoundManager.Instance.SetAudioVolume(EAudioMixerType.SfxInGame, value);
    }
}