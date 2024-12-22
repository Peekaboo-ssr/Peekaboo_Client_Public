using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_DIFFSELECT : MonoBehaviour
{
    [SerializeField] TMP_Text DiffName;
    [SerializeField] private float duration = 0.5f;

    private uint EASYID = 101;
    private uint NORMALID = 102;
    private uint HARDID = 103;
    public void ChangeDiffToEASY()
    {
        GameManager.Instance.DiffId = EASYID;
        DiffName.DOText("", duration, false).SetAutoKill(true).OnComplete(() => {
            DiffName.DOText("EASY", duration, false).SetAutoKill(true);
        });
        
    }

    public void ChangeDiffToNORMAL()
    {
        GameManager.Instance.DiffId = NORMALID;
        DiffName.DOText("", duration, false).SetAutoKill(true).OnComplete(() => {
            DiffName.DOText("NORMAL", duration, false).SetAutoKill(true);
        });
    }

    public void ChangeDiffToHARD()
    {
        GameManager.Instance.DiffId = HARDID;
        DiffName.DOText("", duration, false).SetAutoKill(true).OnComplete(() => {
            DiffName.DOText("HARD", duration, false).SetAutoKill(true);
        });
    }

    public void DiffSelectRequest()
    {
        GamePacket packet = new GamePacket();
        packet.DifficultySelectRequest = new C2S_DifficultySelectRequest();
        packet.DifficultySelectRequest.DifficultyId = GameManager.Instance.DiffId;
        GameServerSocketManager.Instance.Send(packet);
        UIManager.Instance.OpenHUDUI();
    }
}
