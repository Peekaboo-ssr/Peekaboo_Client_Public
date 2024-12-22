using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GhostIdleState : EntityBaseState
{
    protected Ghost _ghost;
    public float IdleTimer { get; private set; }
    private float _lastTime;

    public GhostIdleState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        _ghost = stateMachine.Entity as Ghost;
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.StateMachine.AttackState.OnPlayerNotDetected();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Idle);
        IdleTimer = _ghost.StatHandler.CurStat.AttackSuccessWaitingTime;
        _lastTime = Time.time;
    }

    public override void Update()
    {
        base.Update();
        if(Time.time - _lastTime >= IdleTimer)
        {
            _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
        }
    }
}
