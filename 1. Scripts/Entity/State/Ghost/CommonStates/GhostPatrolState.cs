using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 배회하는 상태
/// </summary>
public class GhostPatrolState : GhostBaseState
{  
    public GhostPatrolState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (_ghost.StateMachine.BeforeState != _ghost.StateMachine.MoveState
            && _ghost.StateMachine.BeforeState != _ghost.StateMachine.PatrolState)
            _ghost.NetworkHandler.StateChangeRequest(CharacterState.Move);         

        if (_ghost.IsOpeningDoor)
        {
            _ghost.IsOpeningDoor = false;
        }
        else
        {
            SetDestination(SetRandomDirectionAndDuration());
        }
    }

    public override void Update()
    {
        base.Update();
        if (_ghost.Target != null || _ghost.IsPlayerInRange)
        {
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.MoveState);
        }
        else
        {
            Patrol();
        }      
    }

    private void Patrol()
    {
        if (_ghost.Agent.remainingDistance <= _ghost.Agent.stoppingDistance && !_ghost.Agent.pathPending)
        {
            SetDestination(SetRandomDirectionAndDuration());
        }
    }

    /// <summary>
    /// 랜덤한 방향과 지속 시간 설정
    /// </summary>
    private Vector3 SetRandomDirectionAndDuration()
    {
        // 랜덤 방향 선택 (4방향: 앞, 뒤, 좌, 우)
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        Vector3 curDirection = directions[Random.Range(0, directions.Length)];

        // 랜덤 이동 지속 시간 설정
        float moveDuration = Random.Range(5f, 8f);

        return _ghost.transform.position + curDirection * moveDuration;
    }
}
