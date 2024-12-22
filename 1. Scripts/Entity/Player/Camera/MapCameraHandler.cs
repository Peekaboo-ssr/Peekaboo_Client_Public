using UnityEngine;

public class MapCameraHandler : MonoBehaviour
{
    private AudioListener mapAudioListener;

    private void Awake()
    {
        mapAudioListener = GetComponent<AudioListener>();
        DisableAudioListener();
    }

    private void Start()
    {
        GameManager.Instance.Player.EventHandler.OnDieEvent += EnableAudioListener;
        UIManager.Instance.UI_PlAYERDIE.OnDieUIEnd += DisableAudioListener;
    }

    private void EnableAudioListener(Player player)
    {
        mapAudioListener.enabled = true;
    }

    private void DisableAudioListener()
    {
        mapAudioListener.enabled = false;
    }
}
