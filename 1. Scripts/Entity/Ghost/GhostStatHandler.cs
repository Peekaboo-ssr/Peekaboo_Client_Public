using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GhostStat
{
    public float Speed;                     // 이동 속도
    public float AttackRange;               // 공격 범위
    public float AttackCool;                // 공격 쿨타임
    public float Sight;                     // 시야 반경
    public float SightAngle;                // 시야각
    public float Hearing;                   // 게임 시작 후 스폰 시간
    public float SpawnTime;                 // 청각 감지 범위
    public float AttackSuccessWaitingTime;  // 공격 성공 후 Idle 지속 시간
}

public class GhostStatHandler
{
    [SerializeField] public GhostStat BaseStat { get; private set; }    // 기본 스탯
    [SerializeField] public GhostStat CurStat { get; private set; } // 현재 스탯

    /// <summary>
    /// ScriptableObject를 기반으로 스탯 초기화
    /// </summary>
    /// <param name="ghostSO">유령의 ScriptableObject</param>
    public void Init(GhostBaseSO ghostSO)
    {
        BaseStat = new GhostStat();
        CurStat = new GhostStat();

        InitStat(BaseStat, ghostSO);
        InitStat(CurStat, ghostSO);
    }

    /// <summary>
    /// 스탯 초기값 설정
    /// </summary>
    private void InitStat(GhostStat stat, GhostBaseSO ghostSO)
    {
        stat.Speed = ghostSO.Speed;
        stat.AttackRange = ghostSO.AttackRange;
        stat.AttackCool = ghostSO.AttackCool;
        stat.Sight = ghostSO.Sight;
        stat.SightAngle = ghostSO.SightAngle;
        stat.Hearing = ghostSO.Hearing;
        stat.SpawnTime = ghostSO.SpawnTime;
        stat.AttackSuccessWaitingTime = ghostSO.AttackSuccessWaitingTime;
    }

    public void ChangeRunSpeed()
    {
        CurStat.Speed = BaseStat.Speed + 0.4f;
    }

    public void ChangeWalkSpeed()
    {
        CurStat.Speed = BaseStat.Speed;
    }
}
