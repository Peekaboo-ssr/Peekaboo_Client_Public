using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_SoulCredit : MonoBehaviour
{
    [SerializeField] private TMP_Text curCreditText;
    [SerializeField] private TMP_Text targetCreditText;
    [SerializeField] private float changeDelay;
    private int curGold = 0;
    public void UpdateCreditText(int amount)
    {
        curCreditText.DOCounter(curGold, amount, changeDelay, false)
            .SetAutoKill(true)
            .OnComplete(() => { curGold = amount; });
    }

    public void UpdateTargetCreditText(int amount)
    {
        targetCreditText.text = $"/ {amount}";
    }

    public void InitCreditText()
    {
        curCreditText.text = $"200";
        curGold = 200;
    }
}
