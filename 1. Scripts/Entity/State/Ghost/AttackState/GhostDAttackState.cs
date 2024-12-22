using UnityEngine;

public class GhostDAttackState : GhostAttackState
{
    private bool isAttackSuccessful;
    public GhostDAttackState(GhostStateMachine stateMachine) : base(stateMachine)
    {
        _ghost = stateMachine.Entity as GhostD;
    }

    //protected override void PerformAttack()
    //{
    //    // 눈가리기
    //    // 숫자 세기
    //    // 성공시 isAttackSuccessful = true;

    //    if (isAttackSuccessful)
    //    {
    //        //_ghost.Target.StatHandler.TakeDamageRequest(_ghost.GhostId);
    //        _ghost.IsAttackReached = true;
    //        _ghost.Target = null;
    //    }
    //    else
    //    {
    //        float randomIdleTime = Random.Range(60f, 180f); // 1분(60초) ~ 3분(180초)
    //        //_ghost.StateMachine.IdleState.SetIdleTimer(randomIdleTime);
    //        //_ghost.StateMachine.ChangeState(_ghost.StateMachine.IdleState);
    //    }
    //}
}