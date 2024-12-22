public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.NetworkHandler.StateChangeRequest(CharacterState.Attack);
        StartAnimTrigger(entityStateMachine.Entity.AnimationData.AttackParameterHash);

        Attack();
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    private void Attack()
    {
        Ghost targetGhost = GetTarget();
        // targetGhost.Attack();

        // 공격 완료 후 Idle State로 돌아가기
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    private Ghost GetTarget()
    {
        return player.InteractHandler.targetGhost;
    }
}