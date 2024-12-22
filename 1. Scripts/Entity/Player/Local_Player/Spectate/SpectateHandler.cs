using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpectateHandler : MonoBehaviour
{
    // key = UserId, value = RemotePlayer
    private Dictionary<string, RemotePlayer> spectateRemotePlayers = new Dictionary<string, RemotePlayer>();
    // key = selectedIndex, value = UserId
    private Dictionary<int, string> playerIDs = new Dictionary<int, string>();
    private int selectedIndex;

    private void Start()
    {
        Init();
        RemoteManager.Instance.OnPlayerDie += RemotePlayerDie;
    }

    private void Init()
    {
        foreach (var remotePlayer in RemoteManager.Instance.PlayerDictionary)
        {
            if(!remotePlayer.Value.IsDie)
                spectateRemotePlayers[remotePlayer.Key] = remotePlayer.Value;
        }

        int idx = 0;
        foreach (var remotePlayer in spectateRemotePlayers)
        {
            playerIDs[idx] = remotePlayer.Key;
            idx++;
        }

        selectedIndex = 0;
        CameraOn(selectedIndex);
    }

    private void CameraOn(int selectedIdx)
    {
        if (spectateRemotePlayers.ContainsKey(playerIDs[selectedIdx]))
        {
            RemotePlayer remotePlayer = spectateRemotePlayers[playerIDs[selectedIdx]];
            if (remotePlayer != null)
                remotePlayer.OnSpectate();

            UIManager.Instance.UI_SPECTATE.UpdatePlayerId(playerIDs[selectedIdx]);

            //현재 관전중인 플레이어 Obj 기준으로 3D 채널 다시 코루틴 실행
            //VivoxManager.Instance.Update3DChannelObj(spectateRemotePlayers[playerIDs[selectedIdx]].gameObject);
        }
    }

    private void CameraOff(int selectedIdx)
    {
        if (spectateRemotePlayers.ContainsKey(playerIDs[selectedIdx]))
        {
            RemotePlayer remotePlayer = spectateRemotePlayers[playerIDs[selectedIdx]];
            if(remotePlayer !=  null)
                remotePlayer.OffSpectate();
        }        
    }

    public void TranslateCamera(bool isUp)
    {
        // 모든 플레이어가 죽었을 때 관전모드 끄기
        if (spectateRemotePlayers.Count <= 0)
        {
            StopSpectate();
            return;
        }

        CameraOff(selectedIndex);
        if (!isUp)
            selectedIndex = selectedIndex - 1 < 0 ? spectateRemotePlayers.Count - 1 : selectedIndex - 1;
        else
            selectedIndex = selectedIndex + 1 >= spectateRemotePlayers.Count ? 0 : selectedIndex + 1;
        CameraOn(selectedIndex);
    }

    private void RemotePlayerDie(string diedUserId)
    {
        if (spectateRemotePlayers.ContainsKey(diedUserId))
        {
            // 죽은 플레이어의 관전 설정 끄기
            spectateRemotePlayers[diedUserId].OffSpectate();

            bool isSelectedSpectatedPlayer = playerIDs[selectedIndex] == diedUserId;

            // 딕셔너리에서 제거
            spectateRemotePlayers.Remove(diedUserId);
            var deleteKey = playerIDs.FirstOrDefault(pair => pair.Value == diedUserId);
            if(playerIDs.ContainsKey(deleteKey.Key)) 
                playerIDs.Remove(deleteKey.Key);

            // 모든 플레이어가 죽었을 때 관전모드 끄기
            if (spectateRemotePlayers.Count <= 0)
            {
                StopSpectate();
                return;
            }

            // 현재 관전 중인 플레이어가 죽었다면
            if (isSelectedSpectatedPlayer)
            {
                ReindexPlayers();
                selectedIndex = 0; // 새롭게 시작
                TranslateCamera(true);
            }
            else
            {
                string playerId = spectateRemotePlayers[playerIDs[selectedIndex]].UserID;
                ReindexPlayers();
                selectedIndex = playerIDs.FirstOrDefault(pair => pair.Value == playerId).Key;
            }
        }
    }

    private void ReindexPlayers()
    {
        playerIDs.Clear();
        int idx = 0;
        foreach (var player in spectateRemotePlayers)
        {
            playerIDs[idx] = player.Key;
            idx++;
        }
    }

    public void StopSpectate()
    {
        UIManager.Instance.UI_SPECTATE.UpdatePlayerId("");

        OffAllSpectate();
        if (gameObject != null)
            Destroy(gameObject);
    }

    private void OffAllSpectate()
    {
        foreach (var player in spectateRemotePlayers)
        {
            if(player.Value.gameObject.activeInHierarchy)
                player.Value.OffSpectate();
        }
    }
}
