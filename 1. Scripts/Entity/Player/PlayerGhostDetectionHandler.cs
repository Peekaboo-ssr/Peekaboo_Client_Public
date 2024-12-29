using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGhostDetectionHandler : MonoBehaviour
{
    [SerializeField] private Ghost curInRangeGhost;
    private Coroutine ghostDetectionCoroutine;
    private WaitForSeconds detectionWaitForSeconds;

    //Sound
    private float lastSeeGhostTime;
    private float minSeeGhostSoundTime; // 귀신을 보았을 때 사운드의 최소 실행 시간
    private bool nowPlayingGhostBgm;

    private Player player;
    private LocalPlayer localPlayer;
    private Dictionary<uint, (bool, Ghost)> ghostDictionary = new Dictionary<uint,(bool, Ghost)>(); // 보고있는 Ghost면 True
    private HashSet<Ghost> nowSeenGhosts = new HashSet<Ghost>();
    private List<uint> keys;

    private void Start()
    {
        // 게임이 시작될 때 GhostDetection 시작
        GameServerSocketManager.Instance.OnGameStart += StartDetection;
    }

    public void Init(Player player)
    {
        this.player = player;
        detectionWaitForSeconds = new WaitForSeconds(.1f);
        minSeeGhostSoundTime = 5f;
        localPlayer = player as LocalPlayer;
    }

    private void InitGhost()
    {
        ghostDictionary.Clear();
        foreach (var ghost in RemoteManager.Instance.GhostDictionary)
        {
            ghostDictionary[ghost.Key] = new(false, ghost.Value);
        }
        keys = new List<uint>(ghostDictionary.Keys);
    }

    public void StartDetection()
    {
        InitGhost();
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
            if (IsDiePlayer()) yield break;

            List<Collider> hitList = player.FindEntitiesInSight("Ghost", player.PlayerSO.Sight);

            if (hitList != null && hitList.Count > 0) // 보고있는 Ghost가 있을 때
            {
                #region 새롭게 보는 Ghost Dic
                nowSeenGhosts.Clear();
                foreach (var hit in hitList)
                {
                    if (hit == null) 
                        continue;

                    Ghost ghost = hit.GetComponent<Ghost>();
                    if (ghost == null) 
                        continue;

                    // 현재 보고있는 Ghost 리스트에 추가
                    nowSeenGhosts.Add(ghost);

                    if (!ghostDictionary[ghost.GhostId].Item1)
                    {
                        ghostDictionary[ghost.GhostId] = (true, ghostDictionary[ghost.GhostId].Item2);
                        SeeGhost(player, ghostDictionary[ghost.GhostId].Item2.GhostId);
                        PlaySeeGhostSound(ghost.GhostId);
                    }
                       
                    // Sound
                    if (curInRangeGhost == null || hit.gameObject != curInRangeGhost.gameObject)
                        curInRangeGhost = ghost;
                    lastSeeGhostTime = Time.time;
                }
                #endregion

                #region 안보고 있는 Ghost 처리

                for (int i = 0; i < keys.Count; i++)
                {
                    var ghost = ghostDictionary[keys[i]];

                    if (!nowSeenGhosts.Contains(ghost.Item2) && ghost.Item1)
                    {
                        ghostDictionary[keys[i]] = (false, ghost.Item2);
                        NotSeeGhost(ghost.Item2.GhostId);
                    }                        
                }
                #endregion
            }
            else // 보고있는 Ghost가 아예 없을 때
            {
                NotSeeAllGhost();
                PlayNotSeeGhostSound();
            }

            yield return detectionWaitForSeconds;
        }
    }

    private void SeeGhost(Player player, uint ghostId)
    {
        var curGhost = ghostDictionary[ghostId];

        if (curGhost.Item2 != null && curGhost.Item2.EventHandler != null && NetworkManager.Instance.IsHost)
        {
            curGhost.Item2.EventHandler.SeeGhost(player);
        } 
    }

    private void NotSeeGhost(uint ghostId)
    {
        var curGhost = ghostDictionary[ghostId];

        if (curGhost.Item2 != null && curGhost.Item2.EventHandler != null && NetworkManager.Instance.IsHost)
        {
            curGhost.Item2.EventHandler.NotSeeGhost(player);
        }
    }

    private void NotSeeAllGhost()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            var ghost = ghostDictionary[keys[i]];

            if (ghost.Item1 && ghost.Item2 != null && ghost.Item2.EventHandler != null && NetworkManager.Instance.IsHost)
            {
                ghostDictionary[keys[i]] = (false, ghost.Item2);
                ghost.Item2.EventHandler.NotSeeGhost(player);
            }
        }
    }

    private bool IsDiePlayer()
    {
        if (!player.gameObject.activeInHierarchy || player == null)
        {
            NotSeeAllGhost();
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

    private void PlaySeeGhostSound(uint ghostId)
    {
        var curGhost = ghostDictionary[ghostId];
        if(curGhost.Item2 != null)
            curGhost.Item2.PlayAppearSound(curGhost.Item2.GhostType);

        // 사운드 실행 중이지 않고, Local Player일 때 Main Bgm 틀기
        if (!nowPlayingGhostBgm && localPlayer != null)
        {
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
