using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlAYERDIE : MonoBehaviour
{
    [Header("# Die UI")]
    [SerializeField] private Image blackBg;
    [SerializeField] private TextMeshProUGUI TXT_PlayerDie;
    [SerializeField] private TextMeshProUGUI TXT_UpLine;
    [SerializeField] private TextMeshProUGUI TXT_DownLine;

    [Header("# Dotween")]
    [SerializeField] private Vector3 upLineTargetPos;
    [SerializeField] private Vector3 downLineTargetPos;
    private Tween playerDieTxtFade;
    private Tween upLineFade;
    private Tween downLineFade;

    [Header("Spectate")]
    [SerializeField] private GameObject spectatePrefab;

    private float blinkDuration = 0.4f;
    private Coroutine dieUICoroutine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(3f);
    private bool isFirst = true;
    private bool isSpectate = false;

    public event Action OnDieUIEnd;

    void OnEnable()
    {
        isSpectate = false;

        if (!isFirst)
            StartDieUI();
        else
            isFirst = false;
    }

    void OnDisable()
    {
        StopDieUI();
    }

    private void Reset()
    {
        TXT_UpLine.transform.localPosition = new Vector3(0, 400, 0);
        TXT_DownLine.transform.localPosition = new Vector3(0, -400, 0);

        TXT_UpLine.alpha = 1;
        TXT_DownLine.alpha = 1;
        TXT_PlayerDie.alpha = 1;
    }

    private void StartDieUI()
    {
        StopDieUI();
        dieUICoroutine = StartCoroutine(DieUICoroutine());
    }

    private void StopDieUI()
    {
        if(dieUICoroutine != null) 
            StopCoroutine(dieUICoroutine);
    }

    private IEnumerator DieUICoroutine()
    {
        SoundManager.Instance.PlayBgm(EBgm.Die);
        blackBg.gameObject.SetActive(true);

        yield return null;

        TXT_UpLine.gameObject.SetActive(true);
        TXT_DownLine.gameObject.SetActive(true);

        Reset();

        // Fade
        upLineFade = TXT_UpLine.DOFade(0, blinkDuration).SetLoops(-1, LoopType.Yoyo);
        downLineFade = TXT_DownLine.DOFade(0, blinkDuration).SetLoops(-1, LoopType.Yoyo);
        playerDieTxtFade = TXT_PlayerDie.DOFade(0, blinkDuration).SetLoops(-1, LoopType.Yoyo); // Die Text On

        // DoMove
        TXT_UpLine.transform.DOMove(upLineTargetPos, 6f).OnComplete(() => { 
            TXT_UpLine.gameObject.SetActive(false); 
            upLineFade.Kill();
        });
        TXT_DownLine.transform.DOMove(downLineTargetPos, 6f).OnComplete(() => {
            TXT_DownLine.gameObject.SetActive(false); 
            downLineFade.Kill();          
        });

        TXT_PlayerDie.gameObject.SetActive(true);

        yield return waitForSeconds;

        blackBg.gameObject.SetActive(false);
        Spectate();

        yield return waitForSeconds;

        TXT_PlayerDie.gameObject.SetActive(false);
        playerDieTxtFade.Kill();

        GameManager.Instance.CanRestartGame = true;
        SoundManager.Instance.StopBgm();
        OnDieUIEnd?.Invoke();

        yield return waitForSeconds;

        if(isSpectate)
            UIManager.Instance.OpenSpectateUI();
        else 
            UIManager.Instance.CloseUI();
    }

    #region Spectate
    private async void Spectate()
    {
        if (RemoteManager.Instance.IsAliveRemotePlayer())
        {
            // 한 명이라도 남아있으면 관전 시작
            Instantiate(spectatePrefab);

            // Vivox 채널 재접속 
            await VivoxManager.Instance.JoinVoiceChannel(NetworkManager.Instance.GameSessionId + "D");
            VivoxManager.Instance.VoiceOnlyDieChannel();

            isSpectate = true;

        }
    }
    #endregion
}
