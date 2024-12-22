public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.NetworkHandler.StateChangeRequest(CharacterState.Idle);
        entityStateMachine.UpdateCharacterState(CharacterState.Idle);
        StartAnimation(entityStateMachine.Entity.AnimationData.IdleParameterHash);

        player.StopFootStepSound();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(entityStateMachine.Entity.AnimationData.IdleParameterHash);
    }
}