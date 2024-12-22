using UnityEngine;

public class PlayerStateMachine : EntityStateMachine
{
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    public PlayerStateMachine(Player player)
    {
        Entity = player;

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        RunState = new PlayerRunState(this);
        AttackState = new PlayerAttackState(this);
    }
}