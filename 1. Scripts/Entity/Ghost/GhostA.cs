using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostA : Ghost
{
    private HashSet<Player> seePlayers;
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

                seePlayers = new HashSet<Player>();

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
        Debug.Log($"Ghost{GhostId} 귀신 처다봄 : {player.name}");
        seePlayers.Add(player);
        if (seePlayers.Count > 1) return;
        Debug.Log($"Ghost{GhostId} 귀신 처다보는 사람 있음");
        GameServerSocketManager.Instance.Send(CreateSeePacket(true));
    }

    protected void NotSeeGhostRequest(Player player)
    {
        Debug.Log($"Ghost{GhostId} 귀신 안처다봄 : {player.name}");
        if (!seePlayers.Remove(player)) return;
        Debug.Log($"Ghost{GhostId} 귀신 안처다봄 유효한값 : {player.name}");
        if (seePlayers.Count > 0) return;
        Debug.Log($"Ghost{GhostId} 귀신 처다보는 사람 없음");
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
