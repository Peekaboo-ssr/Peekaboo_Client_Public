using UnityEngine;

public class GhostAStateMachine : GhostStateMachine
{
    public GhostTriggerAttackState AAttackState { get; private set; }
    public GhostAStateMachine(Ghost ghost) : base(ghost)
    {
        Entity = ghost as GhostA;
        AAttackState = new GhostTriggerAttackState(this);

        AttackState = AAttackState;
    }
}
