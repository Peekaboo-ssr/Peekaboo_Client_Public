using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EXIT : MonoBehaviour
{
    [Header("# Exit UI")]
    [SerializeField] private Image mainBg;
    [SerializeField] private Image borderBg1;
    [SerializeField] private Image borderBg2;
    [SerializeField] private Image borderAlivePlayerNums;
    [SerializeField] private Image borderBg3;
    [SerializeField] private Image borderRemainDay;
    [SerializeField] private TextMeshProUGUI TXT_AlivePlayerNums;
    [SerializeField] private TextMeshProUGUI TXT_RemainDay;

    private Coroutine exitUICoroutine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
    private bool isFirst= true;

    void OnEnable()
    {
        if (!isFirst)
            StartExitUI();
        else
            isFirst = false;
    }

    void OnDisable()
    {
        StopExitUI();
    }

    private void Reset()
    {
        mainBg.gameObject.SetActive(false); 
        borderBg1.gameObject.SetActive(false);
        borderBg2.gameObject.SetActive(false);
        borderBg3.gameObject.SetActive(false);
        borderAlivePlayerNums.gameObject.SetActive(false);
        borderRemainDay.gameObject.SetActive(false);
    }

    private void StartExitUI()
    {
        StopExitUI();
        exitUICoroutine = StartCoroutine(ExitUICoroutine());
    }

    private void StopExitUI()
    {
        if (exitUICoroutine != null)
            StopCoroutine(exitUICoroutine);
    }

    private IEnumerator ExitUICoroutine()
    {
        Reset();

        int totalPlayerNum = RemoteManager.Instance.PlayerDictionary.Count + 1;
        int alivePlayerNum = GetAlivePlayerNum();

        mainBg.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderBg1.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderBg2.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderAlivePlayerNums.gameObject.SetActive(true);
        TXT_AlivePlayerNums.text = $"{alivePlayerNum} / {totalPlayerNum}";
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderBg3.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderRemainDay.gameObject.SetActive(true);
        TXT_RemainDay.text = $"{GameManager.Instance.RemainingDay}일";
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        UIManager.Instance.OpenHUDUI();
    }

    private int GetAlivePlayerNum()
    {
        int count = RemoteManager.Instance.GetAliveRemotePlayerNum();

        if (!GameManager.Instance.Player.StatHandler.IsDie())
            count++;

        return count;
    }
}
