using UnityEngine;
using System;

public class PlayerEventHandler : MonoBehaviour
{ 
    public event Action<Vector3> OnMoveEvent;
    public event Action<Vector3> OnLookEvent;
    public event Action OnJumpEvent;
    public event Action<EInteractType> OnInteractEvent;
    public event Action OnHitEvent;
    public event Action<Player> OnDieEvent;

    public void CallMoveEvent(Vector3 value)
    {
        OnMoveEvent?.Invoke(value);
    }

    public void CallLookEvent(Vector3 value)
    {
        OnLookEvent?.Invoke(value);
    }

    public void CallJumpEvent()
    {
        OnJumpEvent?.Invoke();
    }

    public void CallInteractEvent(EInteractType type)
    {
        OnInteractEvent?.Invoke(type);
    }

    public void CallHitEvent()
    {
        OnHitEvent?.Invoke();
    }

    public void CallDieEvent(Player player)
    {
        OnDieEvent?.Invoke(player);
    }
}