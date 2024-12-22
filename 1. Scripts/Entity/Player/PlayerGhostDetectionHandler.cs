using UnityEngine;
using System.Collections;

public class PlayerGhostDetectionHandler : MonoBehaviour
{
    private Player player;
    [SerializeField] private Ghost curGhost;
    [SerializeField] private Ghost curInRangeGhost;
    private Coroutine ghostDetectionCoroutine;
    private WaitForSeconds detectionWaitForSeconds;

    //Sound
    private float lastSeeGhostTime;
    private float minSeeGhostSoundTime; // 귀신을 보았을 때 사운드의 최소 실행 시간
    private bool nowPlayingGhostBgm;

    public void Init(Player player)
    {
        this.player = player;
        detectionWaitForSeconds = new WaitForSeconds(.1f);
        minSeeGhostSoundTime = 5f;
    }

    private void Start()
    {
        // 게임이 시작될 때 GhostDetection 시작
        GameServerSocketManager.Instance.OnGameStart += StartDetection;
    }

    public void StartDetection()
    {
        StopDetection();
        ghostDetectionCoroutine = StartCoroutine(DetectGhost());
    }

    public void StopDetection()
    {
        if(ghostDetectionCoroutine != null)
        {
            StopCoroutine(ghostDetectionCoroutine);
        }
    }

    private IEnumerator DetectGhost()
    {
        LocalPlayer localPlayer = player as LocalPlayer;
        while (true)
        {           
            if (isDiePlayer()) yield break;
            Collider hit = player.FindEntityInSight("Ghost", player.PlayerSO.Sight);
            if(hit != null)
            {
                if(curGhost == null || hit.gameObject != curGhost.gameObject)
                {
                    curGhost = hit.GetComponent<Ghost>();
                    if (curGhost.EventHandler == null) continue;
                    if (isDiePlayer()) yield break;
                    curGhost.EventHandler.SeeGhost(player);
                    
                    if (curInRangeGhost == null || hit.gameObject != curInRangeGhost.gameObject)
                        curInRangeGhost = hit.GetComponent<Ghost>();

                    // 이미 실행중일 땐 새로 실행하지 않도록 함
                    if (!nowPlayingGhostBgm && localPlayer != null)
                    {                       
                        SoundManager.Instance.PlayBgm(EBgm.SeeGhost);
                        SoundManager.Instance.PlayHeartBeatBgm(EHeartBeatBgm.HeartBeat);
                        nowPlayingGhostBgm = true;
                    }
                    lastSeeGhostTime = Time.time;
                }
            }
            else
            {
                // 귀신과 일정 거리 이상 떨어졌을 때 
                if (nowPlayingGhostBgm && !IsInRange() && localPlayer != null) 
                {
                    // 최소 브금 실행 시간을 넘기면 Bgm 끄기
                    if(Time.time - lastSeeGhostTime >= minSeeGhostSoundTime)
                    {
                        SoundManager.Instance.PlayBgm(EBgm.InGame);
                        SoundManager.Instance.StopHeartBeatBgm();
                        nowPlayingGhostBgm = false;
                        curInRangeGhost = null;
                    }
                }
                if (isDiePlayer()) yield break;            
                NotSeeGhost();
            }
            yield return detectionWaitForSeconds;
        }
    }

    private bool isDiePlayer()
    {
        if (!player.gameObject.activeInHierarchy || player == null)
        {
            NotSeeGhost();
            return true;
        }
        return false;
    }

    private void NotSeeGhost()
    {
        if (curGhost != null && curGhost.EventHandler != null)
        {
            curGhost.EventHandler.NotSeeGhost(player);
            curGhost = null;
        }
    }

    private bool IsInRange()
    {
        if(curInRangeGhost == null)
            return true;

        float distance = Vector3.SqrMagnitude(curInRangeGhost.transform.position - transform.position);
        float ghostSight = curInRangeGhost.GhostSO.Sight;
        return distance < ghostSight * ghostSight;
    }
}
