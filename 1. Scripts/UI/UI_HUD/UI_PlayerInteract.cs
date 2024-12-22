using TMPro;
using UnityEngine;

public class UI_PlayerInteract : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TXT_Interact;

    private void Start()
    {
        UIManager.Instance.UI_HUD.UI_PlayerInteract = this;
    }

    public void ShowInteractText(string str)
    {
        TXT_Interact.gameObject.SetActive(true);
        TXT_Interact.text = str;
    }

    public void HideInteractText()
    {
        TXT_Interact.gameObject.SetActive(false);
    }
}
