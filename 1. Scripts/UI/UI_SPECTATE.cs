using TMPro;
using UnityEngine;

public class UI_SPECTATE : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_PlayerNickName;

    public void UpdatePlayerNickName(string nickName)
    {
        txt_PlayerNickName.text = nickName;
    }
}
