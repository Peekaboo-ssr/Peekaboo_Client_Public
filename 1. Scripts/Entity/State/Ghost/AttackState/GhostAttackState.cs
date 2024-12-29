using UnityEngine;
using Cysharp.Threading.Tasks;

public class GhostAttackState : GhostBaseState
{
    private float _attackCooldown = 2.0f; // 공격 쿨타임 디폴트값
    private float _lastAttackTime;      // 마지막 공격 시간
    private bool _isAttacking;

    public GhostAttackState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.IsAttackReached = false;
        _ghost.IsDoneAttackAnim = false;
        _isAttacking = false;

        _attackCooldown = _ghost.StatHandler.CurStat.AttackCool;
        if (_attackCooldown <= 0)
        {
            HandleAttackSuccess();
        }
        else if(_ghost.IsFirstAttack)
        {
            _ghost.IsFirstAttack = false;
            TryAttack();
        }      
    }

    public override void Update()
    {
        base.Update();

        if (_ghost.IsAttackReached || _ghost.IsDoneAttackAnim)
        {
            HandleAttack();
        }
        else if (Time.time - _lastAttackTime >= _attackCooldown && !_isAttacking)
        {
            TryAttack();
        }
    }

    private void HandleAttack()
    {
        if (_ghost.IsAttackReached)
        {
            HandleAttackSuccess();
        }
        else
        {
            if (IsPlayerWithinAttackRange() && _ghost.IsEntityInView(_ghost.TargetCollider, _sight))
            {
                _ghost.IsDoneAttackAnim = false;
                _isAttacking = false;
                _lastAttackTime = Time.time;
            }
            else
            {
                _ghost.StateMachine.ChangeState(_ghost.StateMachine.MoveState);
            }          
        }
    }

    private void TryAttack()
    {
        if (!_ghost.Target.gameObject.activeInHierarchy)
        {
            _ghost.OnPlayerNotDetected();
            _ghost.StatHandler.ChangeWalkSpeed();
            _ghost.Agent.speed = _ghost.StatHandler.CurStat.Speed;
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
        }
        else
        {
            _isAttacking = true;
            _ghost.NetworkHandler.StateChangeRequest(CharacterState.Attack);
        }
    }

    private void HandleAttackSuccess()
    {
        PerformAttack();
        _ghost.StatHandler.ChangeWalkSpeed();
        _ghost.Agent.speed = _ghost.StatHandler.CurStat.Speed;
        _ghost.IsFirstAttack = true;
        _ghost.StateMachine.ChangeState(_ghost.StateMachine.IdleState);
    }

    protected virtual void PerformAttack()
    {
    }
}