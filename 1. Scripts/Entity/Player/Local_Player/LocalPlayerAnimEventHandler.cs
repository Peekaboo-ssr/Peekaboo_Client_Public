using UnityEngine;

public class LocalPlayerAnimEventHandler : MonoBehaviour, IAnimationEvent
{
    public void OnJumpStart()
    {
        GameManager.Instance.Player.NetworkHandler.StateChangeRequest(CharacterState.Jump);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.JumpStart, GameManager.Instance.Player.transform.position);
    }

    public void OnJumpEnd()
    {
        GameManager.Instance.Player.NetworkHandler.StateChangeRequest(GameManager.Instance.Player.StateMachine.CharacterState);
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.JumpLand, GameManager.Instance.Player.transform.position);
    }

    public void OnAttackStart()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerAttack, GameManager.Instance.Player.transform.position);
    }

    public void OnHitStart()
    {
        GameManager.Instance.Player.NetworkHandler.StateChangeRequest(CharacterState.Hit);
        GameManager.Instance.Player.HitSound(GameManager.Instance.Player.transform);
    }

    public void OnAnimationEnd()
    {
        GameManager.Instance.Player.NetworkHandler.StateChangeRequest(GameManager.Instance.Player.StateMachine.CharacterState);
    }

    public void OnDie() { }
}