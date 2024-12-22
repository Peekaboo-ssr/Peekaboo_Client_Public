using Cysharp.Threading.Tasks;
using Michsky.UI.Dark;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainPanelManager mainPanelManager;

    #region UI객체 List
    [field: SerializeField] public UI_HUD UI_HUD { get; private set; }
    [field: SerializeField] public UI_STORE UI_STORE { get; private set; }
    [field: SerializeField] public UI_EXTRACT UI_EXTRACT { get; private set; }
    [field: SerializeField] public UI_PlAYERDIE UI_PlAYERDIE { get; private set; }
    [field: SerializeField] public UI_SPECTATE UI_SPECTATE { get; private set; }
    [field: SerializeField] public UI_DDAY UI_DDAY { get; private set; }
    [field: SerializeField] public UI_DIFFSELECT UI_DIFFSELECT { get; private set; }
    #endregion
    #region UI Panel Name List
    [field: SerializeField] public string UI_STRING { get; private set; }
    [field: SerializeField] public string UI_HUD_STRING { get; private set; }
    [field: SerializeField] public string UI_STORE_STRING { get; private set; }
    [field: SerializeField] public string UI_EXTRACT_STRING { get; private set; }
    [field: SerializeField] public string UI_SPECTATE_STRING { get; private set; }
    [field: SerializeField] public string UI_SETTING_STRING {  get; private set; }
    [field: SerializeField] public string UI_EXIT_STRING { get; private set; }
    [field: SerializeField] public string UI_DIE_STRING { get; private set; }
    [field: SerializeField] public string UI_DDAY_STRING { get; private set; }
    [field: SerializeField] public string UI_DIFFSELECT_STRING { get; private set; }
    [field: SerializeField] public string UI_SPECTATESETTING_STRING { get; private set; }
    #endregion

    private void Start()
    {
        OpenHUDUI();
    }

    public void CloseUI()
    {
        mainPanelManager.OpenPanel(UI_STRING);
        GameManager.Instance.Player.InputHandler.UnLockCursor();
    }
    public void OpenHUDUI()
    {
        mainPanelManager.OpenPanel(UI_HUD_STRING);
        GameManager.Instance.Player.InputHandler.UnLockCursor();
    }
    public void OpenStoreUI()
    {
        UI_STORE.Init();
        mainPanelManager.OpenPanel(UI_STORE_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenExtractUI()
    {
        mainPanelManager.OpenPanel(UI_EXTRACT_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenSpectateUI()
    {
        mainPanelManager.OpenPanel(UI_SPECTATE_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenSettingUI()
    {
        mainPanelManager.OpenPanel(UI_SETTING_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenExitUI()
    {
        mainPanelManager.OpenPanel(UI_EXIT_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenDieUI()
    {
        mainPanelManager.OpenPanel(UI_DIE_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenDDayUI()
    {
        mainPanelManager.OpenPanel(UI_DDAY_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenDiffSelectUI()
    {
        mainPanelManager.OpenPanel(UI_DIFFSELECT_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
    public void OpenSpectateSettingUI()
    {
        mainPanelManager.OpenPanel(UI_SPECTATESETTING_STRING);
        GameManager.Instance.Player.InputHandler.LockCursor();
    }
}
