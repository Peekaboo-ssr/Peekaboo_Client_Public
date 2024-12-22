using UnityEngine;

public class GhostStateMachine : EntityStateMachine
{
    public GhostPatrolState PatrolState { get; protected set; }
    public GhostMoveState MoveState { get; protected set; }
    public GhostAttackState AttackState { get; protected set; }
    public GhostIdleState IdleState { get; protected set; }
    public GhostOpenDoorState OpenDoorState { get; protected set; }
    public GhostFailSessionState FailSeeionState { get; protected set; }
    public GhostStateMachine(Ghost ghost)
    {
        Entity = ghost;
        PatrolState = new GhostPatrolState(this);
        MoveState = new GhostMoveState(this);
        AttackState = new GhostAttackState(this);
        IdleState = new GhostIdleState(this);
        OpenDoorState = new GhostOpenDoorState(this);
        FailSeeionState = new GhostFailSessionState(this);
    }
}
