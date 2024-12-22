using TMPro;
using UnityEngine;

public class UI_SPECTATE : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_PlayerId;

    public void UpdatePlayerId(string id)
    {
        txt_PlayerId.text = id;
    }
}
