using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GhostC : Ghost
{
    private WaitForSeconds checkDistanceTime = new WaitForSeconds(0.1f);
    private UnityAction OnDistanceTarget;
    private bool IsPlayerLookAtMe;
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
                IsPlayerLookAtMe = false;

                EventHandler.OnSeeEvent += SeeGhostRequest;
                EventHandler.OnNotSeeEvent += NotSeeGhostRequest;
            }
            else
            {
                StateMachine = new GhostCStateMachine(this);
                StateMachine.ChangeState(StateMachine.FailSeeionState);
            }
        }
        OnDistanceTarget += () => Animator.speed = 1;
    }

    #region 귀신 처다보기 처리
    private void SeeGhostRequest(Player player)
    {
        if (Target == null)
        {
            IsPlayerLookAtMe = true;
            IsPlayerInRange = false;
            TargetCollider = player.GetComponent<Collider>();
            Target = player;
            StateMachine.ChangeState(StateMachine.MoveState);
            GameServerSocketManager.Instance.Send(CreateSeePacket(true));
        }
        else if (Target == player && !IsPlayerLookAtMe)
        {
            IsPlayerLookAtMe = true;
            GameServerSocketManager.Instance.Send(CreateSeePacket(true));
        }
    }

    private void NotSeeGhostRequest(Player player)
    {
        if (Target == player && IsPlayerLookAtMe)
        {
            IsPlayerLookAtMe = false;
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
        if (NetworkManager.Instance.IsHost)
        {
            Agent.isStopped = true;
            StartCoroutine(CheckDistanceCoroutine());
        }
    }

    private IEnumerator CheckDistanceCoroutine()
    {
        while (true)
        {
            if(Target == null || !Target.gameObject.activeInHierarchy)
            {
                OnDistanceTarget?.Invoke();
                ChangePatrol();
                break;
            }

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance > Target.PlayerSO.Sight){
                OnDistanceTarget?.Invoke();
                ChangePatrol();
                break;
            }
            yield return checkDistanceTime;
        }
    }

    private void ChangePatrol()
    {      
        Agent.isStopped = false;
        OnPlayerNotDetected();
        StateMachine.ChangeState(StateMachine.PatrolState);
    }
    #endregion
}
