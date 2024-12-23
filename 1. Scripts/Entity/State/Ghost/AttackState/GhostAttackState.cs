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
            PerformAttack();
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.IdleState);
        }
        else
        {
            _lastAttackTime = Time.time;
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
            if (!_ghost.Target.gameObject.activeInHierarchy)
            {
                _ghost.OnPlayerNotDetected();
                _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
            }
            _isAttacking = true;
            _ghost.NetworkHandler.StateChangeRequest(CharacterState.Attack);
        }
    }

    private void HandleAttack()
    {
        if (_ghost.IsAttackReached)
        {
            PerformAttack();
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.IdleState);
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

    protected virtual void PerformAttack()
    {
    }
}