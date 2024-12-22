using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private Vector3 moveDir;

    public PlayerMoveState(EntityStateMachine stateMachine) : base(stateMachine)
    {
        player.EventHandler.OnMoveEvent += GetMoveDir;
    }

    public override void Enter()
    {
        base.Enter();
        player.NetworkHandler.StateChangeRequest(CharacterState.Move);
        entityStateMachine.UpdateCharacterState(CharacterState.Move);
        StartAnimation(entityStateMachine.Entity.AnimationData.MoveParameterHash);
        player.StartFootStepSound(false);

        player.StatHandler.Walk();
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(entityStateMachine.Entity.AnimationData.MoveParameterHash);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
    }

    private void Move()
    {
        // 플레이어 현재 위치 기준 move Dir 벡터만큼 이동 - fixedDeltaTime 기준
        Vector3 curPosition = player.Rigidbody.position;
        Vector3 targetPosition = curPosition +
    (player.Rigidbody.transform.forward * moveDir.z + player.Rigidbody.transform.right * moveDir.x) *
    player.StatHandler.CurrentStat.MoveSpeed * Time.fixedDeltaTime;

        player.Rigidbody.MovePosition(targetPosition);
    }

    private void GetMoveDir(Vector3 value)
    {
        moveDir = value;
    }

}