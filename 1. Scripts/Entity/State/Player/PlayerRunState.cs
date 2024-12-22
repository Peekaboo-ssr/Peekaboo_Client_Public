using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    public PlayerRunState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        player.NetworkHandler.StateChangeRequest(CharacterState.Run);
        entityStateMachine.UpdateCharacterState(CharacterState.Run);
        StartAnimation(entityStateMachine.Entity.AnimationData.RunParameterHash);
        player.StartFootStepSound(true);

        player.StatHandler.Run();     
        player.StatHandler.UseStamina();
    }

    public override void Exit()
    {
        StopAnimation(entityStateMachine.Entity.AnimationData.RunParameterHash);
    }
}