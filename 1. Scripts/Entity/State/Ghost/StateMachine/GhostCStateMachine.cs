using UnityEngine;

public class GhostCStateMachine : GhostStateMachine
{
    public GhostTriggerAttackState CAttackState { get; private set; }
    public GhostCStateMachine(Ghost ghost) : base(ghost)
    {
        Entity = ghost as GhostC;
        CAttackState = new GhostTriggerAttackState(this);

        AttackState = CAttackState;
    }
}
