using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Noise : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image Image_Mute;

    public void SetNoise(float percentage)
    {
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, percentage, 0.5f);
    }

    public void Mute()
    {
        Image_Mute.gameObject.SetActive(true);
        fillImage.fillAmount = 0;
    }

    public void UnMute()
    {
        Image_Mute.gameObject.SetActive(false);
    }
}
