using UnityEngine;

public class GhostA : Ghost
{
    private int _seePlayerCnt;
    public override void Init(uint ghostId, bool isFail)
    {
        base.Init(ghostId, isFail);

        if (NetworkManager.Instance.IsHost || IsTest)
        {
            if (!isFail)
            {
                StateMachine = new GhostAStateMachine(this);
                StateMachine.ChangeState(StateMachine.PatrolState);
                IsDefeatable = false;

                EventHandler.OnSeeEvent += SeeGhostRequest;
                EventHandler.OnNotSeeEvent += NotSeeGhostRequest;
            }
            else
            {
                StateMachine = new GhostAStateMachine(this);
                StateMachine.ChangeState(StateMachine.FailSeeionState);
            }
        }
    }

    #region 귀신 처다보기 처리
    protected void SeeGhostRequest(Player player)
    {
        _seePlayerCnt++;
        EventHandler.PlayAppearSound(GhostType);

        if (_seePlayerCnt > 1) return;
        GameServerSocketManager.Instance.Send(CreateSeePacket(true));
    }

    protected void NotSeeGhostRequest(Player player)
    {
        if (_seePlayerCnt <= 0) return;
        _seePlayerCnt--;

        if (_seePlayerCnt > 0) return;
        GameServerSocketManager.Instance.Send(CreateSeePacket(false));
    }

    public void SeeGhost()
    {
        Animator.speed = 0;
        if (!NetworkManager.Instance.IsHost) return;
        Agent.isStopped = true;
    }

    public void NotSeeGhost()
    {
        Animator.speed = 1;
        if (!NetworkManager.Instance.IsHost) return;
        Agent.isStopped = false;
    }
    #endregion
}
