using System;
using UnityEngine;

[Serializable]
public class AnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string moveParameterName = "Move";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string runParameterName = "Run";
    [SerializeField] private string dieParameterName = "Die";
    [SerializeField] private string attackParameterName = "Attack";
    [SerializeField] private string hitParameterName = "Hit";
    [SerializeField] private string holdItemParameterName = "HoldItem";

    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int HitParameterHash { get; private set; }
    public int HoldItemParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        DieParameterHash = Animator.StringToHash(dieParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        HitParameterHash = Animator.StringToHash(hitParameterName);
        HoldItemParameterHash = Animator.StringToHash(holdItemParameterName);
    }
}