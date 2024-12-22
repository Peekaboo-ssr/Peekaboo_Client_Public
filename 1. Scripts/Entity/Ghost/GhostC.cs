using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostC : Ghost
{
    public bool IsSeePlayer;

    private WaitForSeconds checkDistanceTime = new WaitForSeconds(0.1f);
    public override void Init(uint ghostId, bool isFail)
    {
        base.Init(ghostId, isFail);

        if (NetworkManager.Instance.IsHost || IsTest)
        {
            if (!isFail)
            {
                StateMachine = new GhostCStateMachine(this);
                StateMachine.ChangeState(StateMachine.PatrolState);
                IsDefeatable = false;

                IsSeePlayer = false;

                EventHandler.OnSeeEvent += SeeGhostRequest;
                EventHandler.OnNotSeeEvent += NotSeeGhostRequest;
            }
            else
            {
                StateMachine = new GhostCStateMachine(this);
                StateMachine.ChangeState(StateMachine.FailSeeionState);
            }
        }
    }

    #region 귀신 처다보기 처리
    private void SeeGhostRequest(Player player)
    {
        if (Target == player)
        {
            IsSeePlayer = true;
            GameServerSocketManager.Instance.Send(CreateSeePacket(true));
        }
    }

    private void NotSeeGhostRequest(Player player)
    {
        if (Target == player)
        {
            IsSeePlayer = false;
            GameServerSocketManager.Instance.Send(CreateSeePacket(false));
        }
    }

    public void SeeGhost()
    {
        Animator.speed = 1;
        if (!NetworkManager.Instance.IsHost) return;
        Agent.isStopped = false;
    }

    public void NotSeeGhost()
    {
        Animator.speed = 0;
        if (NetworkManager.Instance.IsHost) Agent.isStopped = true;
        StartCoroutine(CheckDistanceCoroutine());
    }

    private IEnumerator CheckDistanceCoroutine()
    {
        while (true)
        {
            if(Target == null || !Target.gameObject.activeInHierarchy)
            {         
                ChangePatrol();
                break;
            }

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance > Target.PlayerSO.Sight){
                ChangePatrol();
                break;
            }
            yield return checkDistanceTime;
        }
    }

    private void ChangePatrol()
    {
        Animator.speed = 1;
        if (NetworkManager.Instance.IsHost) Agent.isStopped = false;
        StateMachine.MoveState.OnPlayerNotDetected();
        StateMachine.ChangeState(StateMachine.PatrolState);
    }
    #endregion
}
