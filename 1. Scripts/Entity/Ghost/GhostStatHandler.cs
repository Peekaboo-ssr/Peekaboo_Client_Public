using System;
using UnityEngine;

[Serializable]
public class GhostStat
{
    public float Speed;                     // �̵� �ӵ�
    public float AttackRange;               // ���� ����
    public float AttackCool;                // ���� ��Ÿ��
    public float Sight;                     // �þ� �ݰ�
    public float SightAngle;                // �þ߰�
    public float Hearing;                   // ���� ���� �� ���� �ð�
    public float SpawnTime;                 // û�� ���� ����
    public float AttackSuccessWaitingTime;  // ���� ���� �� Idle ���� �ð�
}

public class GhostStatHandler
{
    [SerializeField] public GhostStat BaseStat { get; private set; }    // �⺻ ����
    [SerializeField] public GhostStat CurStat { get; private set; } // ���� ����

    /// <summary>
    /// ScriptableObject�� ������� ���� �ʱ�ȭ
    /// </summary>
    /// <param name="ghostSO">������ ScriptableObject</param>
    public void Init(GhostBaseSO ghostSO)
    {
        BaseStat = new GhostStat();
        CurStat = new GhostStat();

        InitStat(BaseStat, ghostSO);
        InitStat(CurStat, ghostSO);
    }

    /// <summary>
    /// ���� �ʱⰪ ����
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
    /// Ư�� ���� ����, ���� ���� ���̶� ������ ���� ���̶� �ٸ� ��� �����
    /// </summary>
    /// <param name="statModifier">������ ���� ��</param>
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

        // �� ������ �ּҰ� ����
        CurStat.Speed = Mathf.Max(CurStat.Speed, 0);
        CurStat.AttackRange = Mathf.Max(CurStat.AttackRange, 0);
        CurStat.AttackCool = Mathf.Max(CurStat.AttackCool, 0);
        CurStat.Sight = Mathf.Max(CurStat.Sight, 0);
        CurStat.Hearing = Mathf.Max(CurStat.Hearing, 0);
    }

    /// <summary>
    /// ��� ������ �⺻������ ����
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
