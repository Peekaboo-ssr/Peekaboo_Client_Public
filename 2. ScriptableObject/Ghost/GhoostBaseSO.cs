using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Ghost", fileName = "GhostBaseSO", order = 0)]
public class GhostBaseSO : ScriptableObject
{
    [Header("# 이동속도")]
    public float Speed;

    [Header("# 공격 범위")]
    public float AttackRange;

    [Header("# 공격 쿨타임")]
    public float AttackCool;

    [Header("# 시력")]
    public float Sight;

    [Header("# 시야각도")]
    public float SightAngle = 91.0f;

    [Header("# 청각 감지 범위")]
    public float Hearing;

    [Header("# 게임 시작 후 스폰 시간")]
    public float SpawnTime;

    [Header("# 공격 성공 후 Idle 지속 시간")]
    public float AttackSuccessWaitingTime;
}
