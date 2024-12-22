using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "ItemBaseSO", order = 0)]
public class ItemBaseSO : ScriptableObject
{
    [Header("# ������ �̸�")]
    public string Name;

    [Header("# ������ ����")]
    public int Price;

    [Header("# ��� ���� ����")]
    public bool IsUsable;

    [Header("# ������ ���ӽð�(��)")]
    public float Duration;
}
