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

    [Header("# Text Value")]
    [SerializeField] private TextMeshProUGUI TXT_AlivePlayerNums;
    [SerializeField] private TextMeshProUGUI TXT_RemainDay;
    [SerializeField] private TextMeshProUGUI TXT_SoulDeduction;
    [SerializeField] private TextMeshProUGUI TXT_SoulDeductionValue;

    private Coroutine exitUICoroutine;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
    private bool isFirst= true;
    private Sequence sequence;

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
        TXT_SoulDeduction.gameObject.SetActive(false);
    }

    [ContextMenu("Exit")]
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

        mainBg.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        // Died Player
        ObjectPoolManager.Instance.ResetDiedPlayer();
        yield return waitForSeconds;

        borderBg1.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderBg2.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderAlivePlayerNums.gameObject.SetActive(true);
        TXT_AlivePlayerNums.text = $"{GameManager.Instance.AlivePlayerNum} / {GameManager.Instance.AlivePlayerNum + GameManager.Instance.DiedPlayerNum}";

        SoulDeduction();
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderBg3.gameObject.SetActive(true);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);
        yield return waitForSeconds;

        borderRemainDay.gameObject.SetActive(true);
        TXT_RemainDay.text = $"{GameManager.Instance.RemainingDay}일";
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.ExitPanel, GameManager.Instance.Player.transform.position);

        UIManager.Instance.OpenHUDUI();
    }

    private int GetAlivePlayerNum()
    {
        int count = RemoteManager.Instance.GetAliveRemotePlayerNum();

        if (!GameManager.Instance.Player.StatHandler.IsDie())
            count++;

        return count;
    }

    private void SoulDeduction()
    {
        float deductionNum = GameManager.Instance.SoulDeduction;
        if (deductionNum <= 0) return; // 차감량 없으면 return

        // Soul 차감
        TXT_SoulDeduction.gameObject.SetActive(true);
        TXT_SoulDeductionValue.text = $"- {((int)deductionNum).ToString()}";
        TXT_SoulDeduction.DOFade(1, 0);
        TXT_SoulDeductionValue.DOFade(1, 0); // 즉시 alpha 값 1

        // Dotween
        sequence = DOTween.Sequence();      
        sequence.Join(TXT_SoulDeduction.DOFade(0, 1f).SetEase(Ease.OutQuad).SetDelay(1f));
        sequence.Join(TXT_SoulDeductionValue.DOFade(0, 1f).SetEase(Ease.OutQuad).SetDelay(1f));
        sequence.OnComplete(() => { TXT_SoulDeduction.gameObject.SetActive(false); });
        sequence.Kill();

    }
}
