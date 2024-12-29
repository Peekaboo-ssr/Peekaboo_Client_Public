using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStamina : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Start()
    {
        GameManager.Instance.Player.StatHandler.OnStaminaChangeEvent += UpdateStaminaBar;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        fillImage.fillAmount = 1;
    }

    private void UpdateStaminaBar(float percentage) 
    {
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, percentage, 0.5f);
    }
}
