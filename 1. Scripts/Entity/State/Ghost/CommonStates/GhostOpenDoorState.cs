using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GhostOpenDoorState : EntityBaseState
{
    private Ghost _ghost;
    private float _lastTime;
    public GhostOpenDoorState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        _ghost = stateMachine.Entity as Ghost;
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Idle);
        _ghost.IsOpeningDoor = true;
        _lastTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (Time.time - _lastTime >= 1.0f)
        {
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
