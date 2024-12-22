using UnityEngine;
using UnityEngine.UI;

public class UI_STORE : MonoBehaviour
{
    public Transform Content;
    public StoreItem StoreItemPrefab;
    public Button ExitBtn;
    bool isInit = false;

    public void Init()
    {
        if (isInit) return;
            // DataManager에서 Store에 입력된 item을 기반으로 slot 생성
        ExitBtn.onClick.RemoveAllListeners();
        ExitBtn.onClick.AddListener(Exit);
        foreach (var data in DataManager.Instance.StoreData)
        {
            StoreItem storeItem = Instantiate(StoreItemPrefab, Content);
            storeItem.Init((int)data["ItemTypeId"]);
        }
        isInit = true;
    }

    private void Exit()
    {
        UIManager.Instance.OpenHUDUI();
    }
}
