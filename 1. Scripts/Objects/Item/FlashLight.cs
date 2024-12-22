using UnityEngine;
using System.Collections;
using System;

public class FlashLight : Item
{
    [Header("FlashLight Data")]
    [SerializeField] private Light flashLight;
    [SerializeField] private float maxDuration;
    [SerializeField] private float nowDuration;
    [SerializeField] private bool isFlashOn;

    private LocalPlayer localPlayer;
    private RemotePlayer remotePlayer;
    private Coroutine flashLightCoroutine;

    public event Action<string> OnUseFlashLightEvent;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        maxDuration = ItemData.ItemSO.Duration;
        nowDuration = maxDuration;
        CallUseFlashLightEvent(GetNowDuration());
        isFlashOn = false;
        flashLight.gameObject.SetActive(isFlashOn);
    }

    public override void LocalPickUp()
    {
        base.LocalPickUp();
        localPlayer = GameManager.Instance.Player;
        GameManager.Instance.Player.EventHandler.OnLookEvent += RotateFlashLight;
    }

    public override void LocalThrow()
    {
        base.LocalThrow();
        localPlayer = null;
        GameManager.Instance.Player.EventHandler.OnLookEvent -= RotateFlashLight;
    }

    private void RotateFlashLight(Vector3 lookDir)
    {
        if (flashLight.gameObject == null) return;

        if (flashLight.gameObject.activeSelf)
        {
            Quaternion cameraRotation = GameManager.Instance.Player.CameraTransform.rotation;
            flashLight.transform.rotation = cameraRotation;
        }
    }

    #region remote player
    public override void RemotePickUp(RemotePlayer remotePlayer)
    {
        base.RemotePickUp(remotePlayer);
        this.remotePlayer = remotePlayer;
        remotePlayer.EventHandler.OnLookEvent += RotateRemoteFlashLight;
    }

    public override void RemoteThrow(RemotePlayer remotePlayer)
    {
        base.RemoteThrow(remotePlayer);
        this.remotePlayer = null;
        remotePlayer.EventHandler.OnLookEvent -= RotateRemoteFlashLight;
    }

    private void RotateRemoteFlashLight(Vector3 lookDir)
    {
        if (flashLight.gameObject == null) return;

        if (flashLight.gameObject.activeSelf)
        {
            // Rotation 값을 오일러 값으로 주고받기 때문에, 월드 좌표계의 오일러각으로 설정해주어야함
            flashLight.transform.eulerAngles = lookDir;
        }
    }
    #endregion

    public string GetNowDuration()
    {
        return ((int)nowDuration).ToString();
    }

    public void CallUseFlashLightEvent(string str)
    {
        OnUseFlashLightEvent?.Invoke(str);
    }

    public bool IsFlashOn()
    {
        return isFlashOn;
    }

    public void TurnOn()
    {
        if (nowDuration <= 0)
            return;

        isFlashOn = true;
        flashLight.gameObject.SetActive(isFlashOn);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.FlashLight, transform.position);

        StartFlashLight();
    }

    public void TurnOff()
    {
        if (nowDuration <= 0)
            return;

        isFlashOn = false;
        flashLight.gameObject.SetActive(isFlashOn);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.FlashLight, transform.position);

        StopFlashLight();
    }

    private void StartFlashLight()
    {
        RotateFlashLight(Vector3.zero);
        StopFlashLight();
        flashLightCoroutine = StartCoroutine(FlashLightCoroutine());
    }

    private void StopFlashLight()
    {
        if (flashLightCoroutine != null)
        {
            StopCoroutine(flashLightCoroutine);
        }
    }

    private IEnumerator FlashLightCoroutine()
    {
        while (nowDuration > 0)
        {
            nowDuration -= 1 * Time.deltaTime;
            CallUseFlashLightEvent(GetNowDuration());
            yield return null;
        }

        flashLight.gameObject.SetActive(false);
        isFlashOn = false;
    }

    public override void Destroy()
    {
        if(localPlayer != null) 
            GameManager.Instance.Player.EventHandler.OnLookEvent -= RotateFlashLight;
        if(remotePlayer != null)
             remotePlayer.EventHandler.OnLookEvent -= RotateRemoteFlashLight;

        base.Destroy();
    }

    private void OnDestroy()
    {
        if (localPlayer != null)
            GameManager.Instance.Player.EventHandler.OnLookEvent -= RotateFlashLight;
        if (remotePlayer != null)
            remotePlayer.EventHandler.OnLookEvent -= RotateRemoteFlashLight;
    }
}