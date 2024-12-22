using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 시야 체크
/// </summary>
public class GhostBaseState : EntityBaseState
{
    protected Ghost _ghost;
    protected float _sight;

    public GhostBaseState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        _ghost = stateMachine.Entity as Ghost;
    }

    public override void Enter()
    {
        base.Enter();
        _sight = _ghost.StatHandler.CurStat.Sight;
    }

    public override void Update()
    {
        base.Update();

        DetectPlayerInSight("Player");
        RotateBody();
    }

    /// <summary>
    /// 시야 체크
    /// </summary>
    /// <param name="otherEntityLayer"></param>
    protected void DetectPlayerInSight(string otherEntityLayer)
    {
        if (_ghost.Target == null)
        {
            Collider hit = _ghost.FindEntityInSight(otherEntityLayer, _sight);
            if (hit != null) OnPlayerDetected(hit);
        }
    }

    private void RotateBody()
    {
        if (_ghost.Target == null) return;
        RotateToTarget(_ghost.Target.transform.position);
    }
    private void RotateToTarget(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - _ghost.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        _ghost.transform.rotation = Quaternion.Lerp(_ghost.transform.rotation, targetRot, Time.deltaTime * 10f);
    }

    /// <summary>
    /// 플레이어 시야 내 감지 처리
    /// </summary>
    protected void OnPlayerDetected(Collider hit)
    {
        _ghost.IsPlayerInRange = false;
        _ghost.TargetCollider = hit;
        _ghost.Target = hit.GetComponent<Player>();
    }

    public void OnPlayerNotDetected()
    {
        _ghost.IsPlayerInRange = false;
        _ghost.Target = null;
        _ghost.TargetCollider = null;
    }

    protected void SetDestination(Vector3 destination)
    {
        Vector3 nearestPos = GetNearestNavMeshPos(destination);
        _ghost.Agent.SetDestination(nearestPos);
    }

    protected Vector3 GetNearestNavMeshPos(Vector3 position, float initialDistance = 3.0f, float maxIncrement = 1.0f, float maxLimit = 10.0f)
    {
        NavMeshHit hit;

        // 입력 위치가 이미 유효한 NavMesh 지점인지 확인
        if (NavMesh.SamplePosition(position, out hit, 0f, NavMesh.AllAreas))
        {
            return position;
        }

        float currentDistance = initialDistance;

        // 최대 거리까지 유효한 NavMesh 지점을 찾기 위해 반복
        while (currentDistance <= maxLimit)
        {
            if (NavMesh.SamplePosition(position, out hit, currentDistance, NavMesh.AllAreas))
            {
                return hit.position; // 유효한 위치 반환
            }

            // maxDistance 증가
            currentDistance += maxIncrement;
        }

        return position;
    }

    /// <summary>
    /// 플레이어가 공격 범위 내에 있는지 확인
    /// </summary>
    protected bool IsPlayerWithinAttackRange()
    {
        return Vector3.Distance(_ghost.transform.position, _ghost.Target.transform.position) <= _ghost.StatHandler.CurStat.AttackRange;
    }

    /// <summary>
    /// 플레이어를 따라감
    /// </summary>
    protected void FollowPlayer(IState curState)
    {
        if (_ghost.StateMachine.CurrentState != curState) return;
        if (!IsPlayerWithinAttackRange())
        {
            Debug.Log($"Ghost {_ghost.name}의 타겟 : {_ghost.Target.name}");
            SetDestination(_ghost.Target.transform.position);
        }
        else
        {
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.AttackState);
        }
    }
}
