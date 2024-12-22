using System;
using UnityEngine;

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

    /// <summary>
    /// 특정 스탯 수정, 현재 스탯 값이랑 변경할 스탯 값이랑 다를 경우 변경됨
    /// </summary>
    /// <param name="statModifier">변경할 스탯 값</param>
    public void UpdateStat(GhostStat statModifier)
    {
        if (!Mathf.Approximately(CurStat.Speed, statModifier.Speed))
            CurStat.Speed = statModifier.Speed;

        if (!Mathf.Approximately(CurStat.AttackRange, statModifier.AttackRange))
            CurStat.AttackRange = statModifier.AttackRange;

        if (!Mathf.Approximately(CurStat.AttackCool, statModifier.AttackCool))
            CurStat.AttackCool = statModifier.AttackCool;

        if (!Mathf.Approximately(CurStat.Sight, statModifier.Sight))
            CurStat.Sight = statModifier.Sight;

        if (!Mathf.Approximately(CurStat.SightAngle, statModifier.SightAngle))
            CurStat.SightAngle = Mathf.Clamp(statModifier.SightAngle, 0, 360);

        if (!Mathf.Approximately(CurStat.Hearing, statModifier.Hearing))
            CurStat.Hearing = statModifier.Hearing;

        // 각 스탯의 최소값 보장
        CurStat.Speed = Mathf.Max(CurStat.Speed, 0);
        CurStat.AttackRange = Mathf.Max(CurStat.AttackRange, 0);
        CurStat.AttackCool = Mathf.Max(CurStat.AttackCool, 0);
        CurStat.Sight = Mathf.Max(CurStat.Sight, 0);
        CurStat.Hearing = Mathf.Max(CurStat.Hearing, 0);
    }

    /// <summary>
    /// 모든 스탯을 기본값으로 복원
    /// </summary>
    public void ResetToBaseStat()
    {
        CurStat.Speed = BaseStat.Speed;
        CurStat.AttackRange = BaseStat.AttackRange;
        CurStat.AttackCool = BaseStat.AttackCool;
        CurStat.Sight = BaseStat.Sight;
        CurStat.SightAngle = BaseStat.SightAngle;
        CurStat.Hearing = BaseStat.Hearing;
    }
}
