using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHit : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    private float targetAlpha = .2f;

    public void OnHit()
    {
        Color color = targetImage.color;
        color.a = targetAlpha;
        targetImage.color = color;

        targetImage.DOFade(0f, 1f);
    }
}
