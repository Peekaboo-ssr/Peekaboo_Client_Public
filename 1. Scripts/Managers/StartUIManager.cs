using Michsky.UI.Dark;
using UnityEngine;

public class StartUIManager : Singleton<StartUIManager>
{
    MainPanelManager mainPanelManager;
    [SerializeField] private string UI_HOME_STRING = "Home";
    [SerializeField] private string UI_LOGIN_STRING = "Login";

    protected override void Awake()
    {
        base.Awake();
        mainPanelManager = GetComponent<MainPanelManager>();
    }
    public void InitStartScene()
    {
        if (GameServerSocketManager.Instance.IsLoginSuccess)
            mainPanelManager.OpenPanel(UI_HOME_STRING);
        else
            mainPanelManager.OpenPanel(UI_LOGIN_STRING);
    }
    [ContextMenu("TEST")]
    public void Test()
    {
        mainPanelManager.OpenPanel(UI_HOME_STRING);
    }
}
