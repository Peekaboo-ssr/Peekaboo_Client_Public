using UnityEngine;

public class PlayerBaseState : EntityBaseState
{
    protected LocalPlayer player;
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        player = stateMachine.Entity as LocalPlayer;
        stateMachine = entityStateMachine as PlayerStateMachine;
    }

    public override void Update()
    {
        base.Update();

        if (player.StatHandler.IsDie())
        {
            Stop();
        }
    }

    private void Stop()
    {
        player.StatHandler.Stop();
    }
}
