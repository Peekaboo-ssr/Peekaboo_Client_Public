using UnityEngine;

public class GhostD : Ghost
{
    public override void Init(uint ghostId, bool isFail)
    {
        base.Init(ghostId, isFail);

        if (NetworkManager.Instance.IsHost)
        {
            //StateMachine = new GhostDStateMachine(this);
            //StateMachine.ChangeState((StateMachine as GhostDStateMachine).PatrolState);
        }
        else
        {
            //ChangeAnim(AnimationData.MoveParameterHash, true);
        }
    }
}
