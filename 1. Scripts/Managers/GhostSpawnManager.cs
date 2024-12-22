using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnManager : Singleton<GhostSpawnManager>
{
    private HashSet<int> spawnedGhosts = new HashSet<int>();
    private Dictionary<int, WaitForSeconds> waitSpawnTime = new Dictionary<int, WaitForSeconds>();

    private void Start()
    {
        //CacheWaitForSeconds();
    }

    #region 예전 로직
    /// <summary>
    /// 모든 Ghost 프리팹의 SpawnTime을 미리 캐싱
    /// </summary>
    //private void CacheWaitForSeconds()
    //{
    //    foreach (var ghost in DataManager.Instance.GhostDataDictionary.Values)
    //    {
    //        if (ghost != null && ghost.GhostSO != null)
    //        {
    //            float spawnTime = ghost.GhostSO.SpawnTime;
    //            if (!waitSpawnTime.ContainsKey((int)ghost.GhostType))
    //            {
    //                waitSpawnTime[(int)ghost.GhostType] = new WaitForSeconds(spawnTime);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 단계별로 MinGhostNumber와 MaxGhostNumber 범위에 맞춰 귀신 스폰 (스폰 시간 고려)
    /// 예시 : SpawnGhostsForDifficulty(EDifName.EASY);
    /// </summary>
    //public void SpawnGhostsForDifficulty(EDifName difficulty)
    //{
    //    string difficultyName = difficulty.ToString(); // EASY, NORMAL, HARD
    //    var stageData = DataManager.Instance.StageData;
    //    if (stageData == null)
    //    {
    //        Debug.LogError("StageData == Null. DataManager 체크하세요.");
    //        return;
    //    }

    //    Dictionary<string, object> stageInfo = stageData.Find(data => data["DifName"].ToString() == difficultyName);

    //    if (stageInfo != null)
    //    {
    //        // 데이터 가져오기
    //        string[] spawnGhostCodes = stageInfo["SpawnGhost"].ToString().Split(',');
    //        List<int> spawnGhostTypeIds = new List<int>();
    //        foreach(var spawnGhostCode in spawnGhostCodes)
    //        {
    //            spawnGhostTypeIds.Add(int.Parse(spawnGhostCode));
    //        }
    //        int minGhostNumber = Convert.ToInt32(stageInfo["MinGhostNumber"]);
    //        int maxGhostNumber = Convert.ToInt32(stageInfo["MaxGhostNumber"]);

    //        // 스폰할 귀신 개수 랜덤
    //        int spawnCount = UnityEngine.Random.Range(minGhostNumber, maxGhostNumber + 1);

    //        Debug.Log($"귀신 스폰 개수 : {spawnCount}, 단계 : {difficulty}");

    //        // 스폰 개수만큼 반복
    //        for (int i = 0; i < spawnCount; i++)
    //        {
    //            // 랜덤한 귀신 코드 선택 (중복되지 않은 귀신만 선택)
    //            int ghostTypeId = GetUniqueGhostTypeId(spawnGhostTypeIds);

    //            // 더 이상 스폰 가능한 귀신이 없을 경우 루프 종료
    //            if (ghostTypeId == 0) break;

    //            StartCoroutine(SpawnGhostWithDelay(ghostTypeId));
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError($"데이터 못 불러왔음. 단계 : {difficulty}");
    //    }
    //}

    /// <summary>
    /// 이미 스폰된 귀신을 제외하고 랜덤한 귀신 코드를 반환
    /// </summary>
    //private int GetUniqueGhostTypeId(List<int> spawnGhostTypeIds)
    //{
    //    List<int> availableGhostTypeIds = new List<int>();

    //    foreach (int typeId in spawnGhostTypeIds)
    //    {
    //        if (!spawnedGhosts.Contains(typeId)) // 이미 스폰된 귀신 제외
    //        {
    //            availableGhostTypeIds.Add(typeId);
    //        }
    //    }

    //    if (availableGhostTypeIds.Count > 0)
    //    {
    //        int uniqueGhostTypeId = availableGhostTypeIds[UnityEngine.Random.Range(0, availableGhostTypeIds.Count)];
    //        spawnedGhosts.Add(uniqueGhostTypeId); // 스폰된 귀신 기록
    //        return uniqueGhostTypeId;
    //    }

    //    return 0; // 모든 귀신이 이미 스폰된 경우
    //}

    /// <summary>
    /// 귀신별 스폰 딜레이를 적용하여 스폰
    /// Request용
    /// </summary>
    //private IEnumerator SpawnGhostWithDelay(int ghostTypeId)
    //{
    //    if (!DataManager.Instance.GhostDataDictionary.ContainsKey(ghostTypeId))
    //    {
    //        // If not contain
    //        Debug.Log($"코드에 해당하는 Ghost가 존재하지 않음: {ghostTypeId}");
    //        yield break;
    //    }
    //    if (waitSpawnTime.TryGetValue(ghostTypeId, out WaitForSeconds waitForSeconds))
    //    {
    //        yield return waitForSeconds;
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Ghost {ghostTypeId}의 WaitForSeconds가 캐싱되지 않았습니다.");
    //        yield break;
    //    }

    //    GamePacket packet = new GamePacket();
    //    packet.GhostSpawnRequest = new C2S_GhostSpawnRequest();
    //    packet.GhostSpawnRequest.GhostTypeId = (uint)ghostTypeId;
    //    GameServerSocketManager.Instance.Send(packet);
    //    // 하나 하나 소환 해주고 Request보내고
    //    Debug.Log($"스폰된 Ghost: {ghostTypeId}가 {DataManager.Instance.GhostDataDictionary[ghostTypeId].GhostSO.SpawnTime}초 후 스폰됨");
    //}
    #endregion


}
