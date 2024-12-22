using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class UI_InviteCode : MonoBehaviour
{
    [SerializeField] private float flipTime;
    [SerializeField] private TextMeshProUGUI NormalInviteCodeText;
    [SerializeField] private TextMeshProUGUI HighlightInviteCodeText;
    public void CopyToClipboard()
    {
        UniTask task = UniCopyToClipboard();
    }

    private async UniTask UniCopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = NetworkManager.Instance.InviteCode;
        NormalInviteCodeText.text = "COPIED";
        HighlightInviteCodeText.text = "COPIED";
        await UniTask.Delay(TimeSpan.FromSeconds(flipTime));
        NormalInviteCodeText.text = "INVITECODE";
        HighlightInviteCodeText.text = "INVITECODE";
    }
}