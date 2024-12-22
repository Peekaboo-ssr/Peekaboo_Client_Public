using UnityEngine;

public class GhostDStateMachine : GhostStateMachine
{
    public GhostDAttackState DAttackState { get; private set; }
    public GhostDStateMachine(Ghost ghost) : base(ghost)
    {
        Entity = ghost as GhostD;
        DAttackState = new GhostDAttackState(this);
    }
}
