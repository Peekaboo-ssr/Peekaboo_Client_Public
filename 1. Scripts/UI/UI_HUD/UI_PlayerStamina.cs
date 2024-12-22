using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Start()
    {
        Init();
        GameManager.Instance.Player.StatHandler.OnStaminaChangeEvent += UpdateStaminaBar;
    }

    private void Init()
    {
        fillImage.fillAmount = 1;
    }

    private void UpdateStaminaBar(float percentage) 
    {
        //fillImage.fillAmount = percentage;
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, percentage, 0.5f);
    }
}
