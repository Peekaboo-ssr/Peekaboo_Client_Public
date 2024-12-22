using UnityEngine;

[CreateAssetMenu(menuName = "Item/SoulItem", fileName = "SoulItemBaseSO", order = 0)]
public class SoulItemBaseSO : ItemBaseSO
{
    [Header("# 상점 Value")]
    public int Value;

    [Header("# 난이도 별 배수")]
    public float EasyMultiple;
    public float NormalMultiple;
    public float HardMultiple;

    [Header("# 아이템 최대 스폰 개수")]
    public int MaxItemSpawnNumber;

    [Header("# 아이템 경험치")]
    public int ExpValue;
}
