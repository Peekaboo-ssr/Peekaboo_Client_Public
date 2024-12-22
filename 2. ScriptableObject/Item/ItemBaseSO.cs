using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "ItemBaseSO", order = 0)]
public class ItemBaseSO : ScriptableObject
{
    [Header("# 아이템 이름")]
    public string Name;

    [Header("# 아이템 가격")]
    public int Price;

    [Header("# 사용 가능 여부")]
    public bool IsUsable;

    [Header("# 아이템 지속시간(초)")]
    public float Duration;
}
