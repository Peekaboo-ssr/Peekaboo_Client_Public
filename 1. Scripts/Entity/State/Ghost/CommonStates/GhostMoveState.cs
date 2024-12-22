using Cysharp.Threading.Tasks.Triggers;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GhostMoveState : GhostBaseState
{
    public GhostMoveState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Move);

        if (_ghost.IsOpeningDoor)
        {
            _ghost.IsOpeningDoor = false;
        }
        else
        {
            if (_ghost.IsPlayerInRange)
                SetDestination(_ghost.TargetPos);
        }
    }

    public override void Update()
    {
        base.Update();    

        if (_ghost.Target == null)
        {
            HandleTargetPosMovement();
        }
        else
        {
            if (_ghost.IsEntityInView(_ghost.TargetCollider, _sight))
            {
                FollowPlayer(this);
            }
            else
            {
                UpdateTargetOrSwitchToPatrol();
            }         
        }
    }

    /// <summary>
    /// 첫 번째 타겟이 시야에서 벗어났을 때
    /// </summary>
    private void UpdateTargetOrSwitchToPatrol()
    {
        Collider hit = _ghost.FindEntityInSight("Player", _sight);

        if (hit != null) // 새로운 타겟 존재
        {
            OnPlayerDetected(hit);
        }
        else // 새로운 타겟도 존재X
        {
            if(_ghost.GhostType == EGhostType.E)
            {
                FollowPlayer(this);
            }
            else
            {
                OnPlayerNotDetected();
                _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
            }          
        }        
    }

    /// <summary>
    /// 목표 지점으로 이동 처리
    /// </summary>
    private void HandleTargetPosMovement()
    {
        if (HasReachedDestination())
        {
            OnPlayerNotDetected();
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
        }
    }

    /// <summary>
    /// 목표 지점 도달 여부 확인
    /// </summary>
    private bool HasReachedDestination()
    {
        return !_ghost.Agent.pathPending && _ghost.Agent.remainingDistance <= _ghost.Agent.stoppingDistance;
    }
}
