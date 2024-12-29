using UnityEngine;

public class RemotePlayerAnimEventHandler : MonoBehaviour, IAnimationEvent
{
    public void OnJumpStart()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.JumpStart, transform.position);
    }

    public void OnJumpEnd()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.JumpLand, transform.position);
    }

    public void OnAttackStart()
    {
        SoundManager.Instance.PlayInGameSfx(EInGameSfx.PlayerAttack, transform.position);
    }

    public void OnHitStart()
    {
        GameManager.Instance.Player.HitSound(transform);
    }

    public void OnDie()
    {
        gameObject.transform.parent.gameObject.SetActive(false);

        GameObject corpse = ObjectPoolManager.Instance.SpawnFromPool(EPoolObjectType.DiedPlayer);
        corpse.transform.position = transform.parent.position;
    }

    public void OnAnimationEnd() { }
}
