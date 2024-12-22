using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Player", fileName = "PlayerBaseSO", order = 0)]
public class PlayerBaseSO : ScriptableObject
{
    [Header("# 체력 개수")]
    [Range(1f, 3f)] public int MaxHeart;

    [Header("# 이동속도")]
    public float MoveSpeed;

    [Header("# 달리기 속도")]
    public float RunSpeed;

    [Header("# 점프 쿨타임")]
    public float JumpCoolDown;

    [Header("# 점프 Force")]
    [Range(1f, 6f)] public float JumpForce;

    [Header("# Stamina 총량")]
    public float Stamina;

    [Header("# Stamina 초당 회복량")]
    public float StaminaRecovery;

    [Header("# Stamina 초당 사용량")]
    public float StaminaDrain;

    [Header("# 시야")]
    public float Sight;

    [Header("# 청력")]
    public float Hearing;

    [Header("# 민감도")]
    [Range(0f, 1f)]public float LookSensivity;
}

