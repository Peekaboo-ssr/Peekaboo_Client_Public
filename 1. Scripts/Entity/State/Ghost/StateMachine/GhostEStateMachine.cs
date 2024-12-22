using UnityEngine;

public class GhostEStateMachine : GhostStateMachine
{
    public GhostInactiveState InactiveState { get; private set; }
    public GhostTriggerAttackState EAttackState { get; private set; }
    public GhostEStateMachine(Ghost ghost) : base(ghost)
    {
        Entity = ghost as GhostE;
        InactiveState = new GhostInactiveState(this);
        EAttackState = new GhostTriggerAttackState(this);

        AttackState = EAttackState;
    }
}
