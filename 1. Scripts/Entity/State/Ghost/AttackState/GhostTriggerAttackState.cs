using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GhostTriggerAttackState : GhostAttackState
{
    public GhostTriggerAttackState(EntityStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void PerformAttack()
    {
        base.PerformAttack();
        LocalPlayer localPlayer = _ghost.Target as LocalPlayer;
        RemotePlayer remotePlayer = _ghost.Target as RemotePlayer;
        if (localPlayer != null)
        {
            localPlayer.StatHandler.TakeDamageRequest(_ghost.GhostId);
        }
        else if(remotePlayer != null)
        {
            remotePlayer.NetworkHandler.RemotePlayerAttackedRequest(_ghost.GhostId);
        }
    }
}
