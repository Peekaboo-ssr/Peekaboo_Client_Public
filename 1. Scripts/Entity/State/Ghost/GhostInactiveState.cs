using UnityEngine;

public class GhostInactiveState : EntityBaseState
{
    protected Ghost _ghost;
    public GhostInactiveState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        _ghost = stateMachine.Entity as Ghost;
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Idle);
    }
}
