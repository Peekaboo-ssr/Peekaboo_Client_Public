using UnityEngine;

public class GhostFailSessionState : GhostBaseState
{
    public GhostFailSessionState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _ghost.NetworkHandler.StateChangeRequest(CharacterState.Move);
        _ghost.Target = GameManager.Instance.Player;
        _ghost.TargetCollider = _ghost.Target.GetComponent<Collider>();
    }

    public override void Update()
    {
        base.Update();
        if (_ghost.Target != null)
        {
            FollowPlayer(this);
        }
    }
}
