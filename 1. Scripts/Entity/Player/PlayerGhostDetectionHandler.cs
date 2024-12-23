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

    private LocalPlayer localPlayer;

    public void Init(Player player)
    {
        this.player = player;
        detectionWaitForSeconds = new WaitForSeconds(.1f);
        minSeeGhostSoundTime = 5f;
        localPlayer = player as LocalPlayer;
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
        while (true)
        {           
            if (IsDiePlayer())  yield break; // 플레이어가 죽었을 경우 코루틴 break
            Collider hit = player.FindEntityInSight("Ghost", player.PlayerSO.Sight);

            if(hit != null ) // Ghost를 보고있을 때
            {
                if(curGhost == null) // 최근에 본 Ghost가 없다면, Ghost를 보는 처리를 해준다
                { 
                    curGhost = hit.GetComponent<Ghost>();
                    SeeGhost(player);                   
                    
                    if (curInRangeGhost == null || hit.gameObject != curInRangeGhost.gameObject)
                        curInRangeGhost = hit.GetComponent<Ghost>();
                    lastSeeGhostTime = Time.time;

                    PlaySeeGhostSound();
                }
                else if(curGhost != null && hit.gameObject != curGhost.gameObject) // 최근에 본 고스트가 있는데, 지금 보이는 고스트와 다를 때
                {
                    NotSeeGhost(); // 보던 Ghost NotSee처리
                    curGhost = hit.GetComponent<Ghost>();
                    SeeGhost(player); // 새로 보게 된 Ghost See 처리

                    if (curInRangeGhost == null || hit.gameObject != curInRangeGhost.gameObject)
                        curInRangeGhost = hit.GetComponent<Ghost>();
                    lastSeeGhostTime = Time.time;

                    PlaySeeGhostSound();
                }
            }
            else // hit이 Ghost가 아닐 때 NotSeeGhost
            {
                NotSeeGhost();
                PlayNotSeeGhostSound();
            }
            yield return detectionWaitForSeconds;
        }
    }

    private void SeeGhost(Player player)
    {
        if (curGhost != null && curGhost.EventHandler != null)
        {
            if (NetworkManager.Instance.IsHost)
            {
                curGhost.EventHandler.SeeGhost(player);
            }
        }
    }

    private void NotSeeGhost()
    {
        if (curGhost != null && curGhost.EventHandler != null)
        {
            if (NetworkManager.Instance.IsHost)
            {
                curGhost.EventHandler.NotSeeGhost(player);
            }
            curGhost = null;
        }
    }

    private bool IsDiePlayer()
    {
        if (!player.gameObject.activeInHierarchy || player == null)
        {
            NotSeeGhost();
            return true;
        }
        return false;
    }

    private bool IsInRange()
    {
        if(curInRangeGhost == null)
            return false;

        float distance = Vector3.SqrMagnitude(curInRangeGhost.transform.position - transform.position);
        float ghostSight = curInRangeGhost.GhostSO.Sight;
        return distance < ghostSight * ghostSight;
    }

    private void PlaySeeGhostSound()
    {
        // 사운드 실행 중이지 않고, Local Player이며 curGhost가 null이 아닐 때 
        if (!nowPlayingGhostBgm && localPlayer != null && curGhost != null)
        {
            curGhost.PlayAppearSound(curGhost.GhostType);

            SoundManager.Instance.PlayBgm(EBgm.SeeGhost);
            SoundManager.Instance.PlayHeartBeatBgm(EHeartBeatBgm.HeartBeat);
            nowPlayingGhostBgm = true;
        }
    }

    private void PlayNotSeeGhostSound()
    {
        // 귀신과 일정 거리 이상 떨어졌을 때 
        if (nowPlayingGhostBgm && !IsInRange() && localPlayer != null)
        {
            // 최소 브금 실행 시간을 넘기면 Bgm 끄기
            if (Time.time - lastSeeGhostTime >= minSeeGhostSoundTime)
            {
                SoundManager.Instance.PlayBgm(EBgm.InGame);
                SoundManager.Instance.StopHeartBeatBgm();
                nowPlayingGhostBgm = false;
                curInRangeGhost = null;
            }
        }
    }
}
