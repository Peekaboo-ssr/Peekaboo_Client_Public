using Michsky.UI.Dark;
using UnityEngine;
using UnityEngine.UI;

public class Visual : MonoBehaviour
{
    [SerializeField] private HorizontalSelector windowMode;
    [SerializeField] private Slider brightness;
    [SerializeField] private Slider sensitivity;
    [SerializeField] private HorizontalSelector texture;
    [SerializeField] private HorizontalSelector shadowQuality;

    private void Start()
    {
        SettingManager.Instance.VisualSetting = this;
        Init();
    }

    private void Init()
    {

    }
}