using System;
using TMPro;
using UnityEngine;

public class UI_RemainTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimeText;
    TimeSpan time;
    public void UpdateTimeText(uint remainTime)
    {
        time = TimeSpan.FromSeconds(remainTime);
        TimeText.text = $"{time:mm\\:ss}";
    }

    public void InitTimeText()
    {
        TimeText.text = "-:--";
    }
}
