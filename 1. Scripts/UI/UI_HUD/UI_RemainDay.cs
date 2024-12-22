using TMPro;
using UnityEngine;

public class UI_RemainDay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI remainDayText;

    public void InitDayText()
    {
        remainDayText.text = $"Remain Day : 2";
    }
    public void UpdateRemainDayText(uint remainDay)
    {
        remainDayText.text = $"Remain Day : {remainDay}";
    }
}
