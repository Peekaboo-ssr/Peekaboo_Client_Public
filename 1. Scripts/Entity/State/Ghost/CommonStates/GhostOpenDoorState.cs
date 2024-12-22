using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GhostOpenDoorState : GhostBaseState
{
    private float _lastTime;
    public GhostOpenDoorState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Idle);
        _ghost.Agent.isStopped = true;

        _ghost.IsOpeningDoor = true;
        _lastTime = Time.time;
    }

    public override void Update()
    {
        base.Update();
        if(Time.time - _lastTime >= 1.0f)
        {
            _ghost.Agent.isStopped = false;
            if (_ghost.StateMachine.BeforeState == _ghost.StateMachine.PatrolState)
            {
                _ghost.StateMachine.ChangeState(_ghost.StateMachine.PatrolState);
            }
            else if (_ghost.StateMachine.BeforeState == _ghost.StateMachine.MoveState)
            {
                _ghost.StateMachine.ChangeState(_ghost.StateMachine.MoveState);
            }
        }
    }
}
