using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    #region Animation Layer 
    public void ActivateItemHoldLayer(Player player)
    {
        player.Animator.SetBool(player.AnimationData.HoldItemParameterHash, true);
        SetLayerWeight((int)EPlayerAnimLayer.HoldItem, 1, player);
    }

    public void DeActivateItemHoldLayer(Player player)
    {
        player.Animator.SetBool(player.AnimationData.HoldItemParameterHash, false);
        SetLayerWeight((int)EPlayerAnimLayer.HoldItem, 0, player);
    }

    // weight가 1이면 활성화, 0이면 비활성화
    private void SetLayerWeight(int layerIndex, int weight, Player player)
    {
        player.Animator.SetLayerWeight(layerIndex,weight);
    }
    #endregion

    #region Animation 
    public void StartAnimation(int animatorHash, Player player)
    {
        player.Animator.SetBool(animatorHash, true);
    }

    public void StopAnimation(int animatorHash, Player player)
    {
        player.Animator.SetBool(animatorHash, false);
    }

    public void StartAnimTrigger(int animatorHash, Player player)
    {
        player.Animator.SetTrigger(animatorHash);
    }
    #endregion

}